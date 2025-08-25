using AITechDATA.ResultObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
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
            // توجه: در پروژه شما Getها هم POST هستند، پس از متد HTTP برای حساسیت استفاده نمی‌کنیم.
            // bool isWrite = WritePrefixes.Any(p => actionName.StartsWith(p, StringComparison.OrdinalIgnoreCase));

            // 4) نقش کاربر (تک RoleId)
            var user = httpCtx.User;
            int userRoleId = GetUserRoleId(user);
            int userStudentId = GetUserStudentId(user);
            bool hasAllowedRole = _allowedRoles.Count > 0 && _allowedRoles.Contains(userRoleId);

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

            // 6) برای سایر اکشن‌ها:
            //    اگر نقش مجاز است → عبور
            if (hasAllowedRole)
            {
                await next();
                return;
            }

            //    اگر نقش مجاز نیست → بررسی استثنای «دسترسی به اطلاعات خود کاربر» (userId/teacherId/adminId)
            long currentUserId = GetUserId(user);
            if (currentUserId <= 0)
            {
                context.Result = new JsonResult(new { message = "شما مجوز انجام این عملیات را ندارید." })
                { StatusCode = StatusCodes.Status403Forbidden };
                return;
            }

            var candidateIds = ExtractRelevantIds(context, controllerName);
            bool isSelfAccessByUserId = candidateIds.Any() && candidateIds.All(x => x == currentUserId);

            if (isSelfAccessByUserId)
            {
                await next();
                return;
            }

            // === NEW: استثنای roleId ===
            // اگر در ورودی userId/teacherId/adminId نبود، ولی roleId بود و roleId ورودی == roleId کاربر → اجازه
            if (!candidateIds.Any())
            {
                var requestRoleIds = ExtractRoleIds(context);
                bool isSelfAccessByRoleId = requestRoleIds.Any() && requestRoleIds.All(r => r == userRoleId);

                if (isSelfAccessByRoleId)
                {
                    await next();
                    return;
                }

                var requeststudentIds = ExtractStudentIds(context);
                bool isSelfAccessByStudentId = requeststudentIds.Any() && requeststudentIds.All(r => r == userStudentId);

                if (isSelfAccessByStudentId)
                {
                    await next();
                    return;
                }
            }
            // === END NEW ===

            context.Result = new JsonResult(new BitResultObject() { ErrorMessage = "شما مجوز انجام این عملیات را ندارید.",Status= false })
            { StatusCode = StatusCodes.Status403Forbidden };
        }

        // ===== Helpers =====
        private static int GetUserRoleId(ClaimsPrincipal user) 
        { 
            var roleIdClaim = user.FindFirst("Role");
            if (roleIdClaim == null || !int.TryParse(roleIdClaim.Value, out var roleId))
            {
                return 0;
            }
            return int.Parse(roleIdClaim.Value); 
        
        }

        private static int GetUserStudentId(ClaimsPrincipal user)
        {
            var studentIdClaim = user.FindFirst("StudentId");
            if (studentIdClaim == null || !int.TryParse(studentIdClaim.Value, out var roleId))
            {
                return 0;
            }
            return int.Parse(studentIdClaim.Value);

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
        /// - پارامترهای اکشن: userId / teacherId / adminId
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
                   || name.Equals("adminId", StringComparison.OrdinalIgnoreCase)
                   || (isUsersController && name.Equals("id", StringComparison.OrdinalIgnoreCase));
        }

        // === NEW: استخراج roleId از ورودی (پارامتر ساده یا داخل DTO/لیست) ===
        private static HashSet<int> ExtractRoleIds(ActionExecutingContext ctx)
        {
            var result = new HashSet<int>();

            foreach (var (argName, argVal) in ctx.ActionArguments)
            {
                if (argVal is null) continue;

                if (IsSimple(argVal))
                {
                    if (IsRoleKey(argName) && TryToInt(argVal, out var v))
                        result.Add(v);
                    continue;
                }

                ScanObjectForRoleId(argVal, result);
            }

            return result;
        }

        private static HashSet<long> ExtractStudentIds(ActionExecutingContext ctx)
        {
            var result = new HashSet<long>();

            foreach (var (argName, argVal) in ctx.ActionArguments)
            {
                if (argVal is null) continue;

                if (IsSimple(argVal))
                {
                    if (IsStudentKey(argName) && TryToLong(argVal, out var v))
                        result.Add(v);
                    continue;
                }

                ScanObjectForStudentId(argVal, result);
            }

            return result;
        }

        private static bool IsRoleKey(string name)
        {
            return !string.IsNullOrEmpty(name) &&
                   name.Equals("roleId", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsStudentKey(string name)
        {
            return !string.IsNullOrEmpty(name) &&
                   name.Equals("studentdetailsId", StringComparison.OrdinalIgnoreCase);
        }

        private static void ScanObjectForRoleId(object obj, ISet<int> sink)
        {
            if (obj is null) return;

            if (obj is System.Collections.IEnumerable en && obj is not string)
            {
                foreach (var item in en)
                    if (item != null) ScanObjectForRoleId(item, sink);
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
                    if (IsRoleKey(prop.Name) && TryToInt(val, out var v))
                        sink.Add(v);
                }
                else
                {
                    ScanObjectForRoleId(val, sink);
                }
            }
        }

        private static void ScanObjectForStudentId(object obj, ISet<long> sink)
        {
            if (obj is null) return;

            if (obj is System.Collections.IEnumerable en && obj is not string)
            {
                foreach (var item in en)
                    if (item != null) ScanObjectForStudentId(item, sink);
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
                    if (IsStudentKey(prop.Name) && TryToLong(val, out var v))
                        sink.Add(v);
                }
                else
                {
                    ScanObjectForStudentId(val, sink);
                }
            }
        }
        // === END NEW ===

        private static void ScanObject(object obj, ISet<long> sink, bool isUsersController)
        {
            if (obj is null) return;

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

        private static bool TryToInt(object? o, out int val)
        {
            switch (o)
            {
                case int i: val = i; return true;
                case short s: val = s; return true;
                case long l when l is >= int.MinValue and <= int.MaxValue: val = (int)l; return true;
                case string str when int.TryParse(str, out var p): val = p; return true;
                default:
                    if (o != null && int.TryParse(o.ToString(), out var x)) { val = x; return true; }
                    val = 0; return false;
            }
        }
    }
}
