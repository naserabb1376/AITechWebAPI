using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
namespace AITechWebAPI.Validations
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CheckRoleBaseAttribute : Attribute, IAsyncActionFilter
    {
        private readonly int[] _allowedRoles;
        private readonly string[] _sensitiveActions = { "Insert", "Add", "Create", "Edit", "Update", "Delete", "Remove" };

        public CheckRoleBaseAttribute(int[] allowedRoles)
        {
            _allowedRoles = allowedRoles ?? Array.Empty<int>();
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var httpContext = context.HttpContext;

            // دریافت RoleId کاربر
            var roleIdClaim = httpContext.User.FindFirst("Role");
            if (roleIdClaim == null || !int.TryParse(roleIdClaim.Value, out var roleId))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            // گرفتن نام اکشن (مثلاً AddUser, EditProfile, GetList)
            var actionName = context.ActionDescriptor.RouteValues["action"]?.ToLowerInvariant() ?? "";

            // بررسی اینکه آیا این اکشن حساس است یا نه
            bool isSensitive = _sensitiveActions.Any(prefix => actionName.StartsWith(prefix.ToLower()));

            if (!isSensitive)
            {
                await next(); // ادامه بدون بررسی مجوز
                return;
            }

            // بررسی مجوز برای اکشن حساس
            if (!_allowedRoles.Contains(roleId))
            {
                context.Result = new JsonResult(new { message = "شما مجوز انجام این عملیات را ندارید." }) { StatusCode = 403 };
                return;
            }

            await next();
        }
    }

}
