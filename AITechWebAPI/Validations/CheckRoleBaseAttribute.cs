using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net.Http;
using System.Security.Claims;

namespace AITechWebAPI.Validations
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class CheckRoleBaseAttribute : Attribute, IAsyncActionFilter
    {
        private static readonly HashSet<string> WritePrefixes =
            new(StringComparer.OrdinalIgnoreCase) { "Insert", "Add", "Create", "Edit", "Update" };

        private static readonly HashSet<string> DeletePrefixes =
            new(StringComparer.OrdinalIgnoreCase) { "Delete", "Remove" };

        private readonly HashSet<int> _allowedRoles;

        public CheckRoleBaseAttribute(params int[] allowedRoles)
        {
            _allowedRoles = allowedRoles != null ? new HashSet<int>(allowedRoles) : new HashSet<int>();
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var httpCtx = context.HttpContext;

            // 1) AllowAnonymous؟
            if (context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any())
            {
                await next();
                return;
            }

            // 2) احراز هویت
            if (httpCtx?.User?.Identity?.IsAuthenticated != true)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            // 3) نام اکشن/کنترلر
            var actionName = context.ActionDescriptor.RouteValues.TryGetValue("action", out var n) ? (n ?? "") : "";
            var controllerName = context.ActionDescriptor.RouteValues.TryGetValue("controller", out var c) ? (c ?? "") : "";

            bool isDelete = DeletePrefixes.Any(p => actionName.StartsWith(p, StringComparison.OrdinalIgnoreCase));
            bool isWrite = WritePrefixes.Any(p => actionName.StartsWith(p, StringComparison.OrdinalIgnoreCase));
            

            // 4) نقش‌های کاربر
            var user = httpCtx.User;
            var userRoleIds = GetUserRoleIds(user);
            bool hasAllowedRole = _allowedRoles.Count > 0 && (_allowedRoles.Contains(userRoleIds));

            // 5) Delete فقط با نقش مجاز
            if (isDelete)
            {
                if (hasAllowedRole)
                {
                    await next();
                    return;
                }

                context.Result = new JsonResult(new { message = "شما مجوز انجام این عملیات را ندارید." })
                { StatusCode = StatusCodes.Status403Forbidden };
                return;
            }

            // 6) برای عملیات نوشتاری غیر Delete:
            //    اگر نقش مجاز است → عبور
            if (hasAllowedRole)
            {
                await next();
                return;
            }

            //    اگر نقش مجاز نیست → بررسی استثنای «دسترسی به اطلاعات خود کاربر»
            var currentUserId = GetUserId(user);
            if (currentUserId == null)
            {
                context.Result = new JsonResult(new { message = "شما مجوز انجام این عملیات را ندارید." })
                { StatusCode = StatusCodes.Status403Forbidden };
                return;
            }

            var candidateIds = ExtractRelevantIds(context, controllerName);
            // باید تمام ID های یافت شده، برابر با UserId جاری باشند (برای جلوگیری از batch روی دیگری)
            bool isSelfAccess = candidateIds.Any() && candidateIds.All(x => x == currentUserId);

            if (isSelfAccess)
            {
                await next();
                return;
            }

            context.Result = new JsonResult(new { message = "شما مجوز انجام این عملیات را ندارید." })
            { StatusCode = StatusCodes.Status403Forbidden };
        }

        // ===== Helpers =====

        private static int GetUserRoleIds(ClaimsPrincipal user)
        {
            var roleIdClaim = user.FindFirst("Role"); 
            if (roleIdClaim == null || !int.TryParse(roleIdClaim.Value, out var roleId))
            {
                return 0;
            }
            return int.Parse(roleIdClaim.Value);
        }

        private static long GetUserId(ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst("userId");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var roleId))
            {
                return 0;
            }
            return long.Parse(userIdClaim.Value);
        
        }

        /// <summary>
        /// ID های مرتبط در ورودی را استخراج می‌کند:
        /// - پارامترهای اکشن: userId / teacherId
        /// - در کنترلر Users: id
        /// - در مدل‌های پیچیده و کالکشن‌ها به‌صورت بازگشتی.
        /// </summary>
        private static HashSet<long> ExtractRelevantIds(ActionExecutingContext ctx, string controllerName)
        {
            var result = new HashSet<long>();
            bool isUsersController = controllerName.Equals("User", StringComparison.OrdinalIgnoreCase)
                                     || controllerName.Equals("Users", StringComparison.OrdinalIgnoreCase);

            // Route values (در صورت وجود)
            if (isUsersController &&
                ctx.RouteData.Values.TryGetValue("id", out var routeIdObj) &&
                TryToLong(routeIdObj, out var routeId))
            {
                result.Add(routeId);
            }

            // Action arguments
            foreach (var (argName, argVal) in ctx.ActionArguments)
            {
                if (argVal is null) continue;

                // پارامتر ساده؟
                if (IsSimple(argVal))
                {
                    if (IsUserKey(argName, isUsersController) && TryToLong(argVal, out var v))
                        result.Add(v);
                    continue;
                }

                // مدل/آبجکت: بازگشتی
                ScanObject(argVal, result, isUsersController);
            }

            return result;
        }

        private static bool IsUserKey(string name, bool isUsersController)
        {
            if (string.IsNullOrEmpty(name)) return false;
            // id فقط در کنترلر Users معتبر است
            return name.Equals("userId", StringComparison.OrdinalIgnoreCase)
                   || name.Equals("teacherId", StringComparison.OrdinalIgnoreCase)
                   || name.Equals("adminid", StringComparison.OrdinalIgnoreCase)
                   || (isUsersController && name.Equals("id", StringComparison.OrdinalIgnoreCase));
        }

        private static void ScanObject(object obj, ISet<long> sink, bool isUsersController)
        {
            if (obj is null) return;

            // اگر مجموعه است، تک‌تک اعضا را بررسی کن
            if (obj is System.Collections.IEnumerable en && obj is not string)
            {
                foreach (var item in en)
                    if (item != null) ScanObject(item, sink, isUsersController);
                return;
            }

            var type = obj.GetType();
            if (IsSimple(obj)) return;

            foreach (var prop in type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
            {
                if (!prop.CanRead) continue;
                object? val;
                try { val = prop.GetValue(obj); }
                catch { continue; }

                if (val == null) continue;

                if (IsSimple(val))
                {
                    if (IsUserKey(prop.Name, isUsersController) && TryToLong(val, out var v))
                        sink.Add(v);
                }
                else
                {
                    ScanObject(val, sink, isUsersController);
                }
            }
        }

        private static bool IsSimple(object obj)
        {
            var t = obj.GetType();
            return t.IsPrimitive
                   || t.IsEnum
                   || t == typeof(string)
                   || t == typeof(DateTime)
                   || t == typeof(Guid)
                   || t == typeof(decimal)
                   || t == typeof(long)
                   || t == typeof(int)
                   || t == typeof(short)
                   || t == typeof(byte)
                   || t == typeof(ulong)
                   || t == typeof(uint)
                   || t == typeof(ushort)
                   || t == typeof(sbyte);
        }

        private static bool TryToLong(object? o, out long val)
        {
            switch (o)
            {
                case long l: val = l; return true;
                case int i: val = i; return true;
                case short s: val = s; return true;
                case string str when long.TryParse(str, out var p): val = p; return true;
                default:
                    if (o != null && long.TryParse(o.ToString(), out var x)) { val = x; return true; }
                    val = 0; return false;
            }
        }
    }
}
