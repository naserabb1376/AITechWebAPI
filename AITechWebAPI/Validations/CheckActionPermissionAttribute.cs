using AITechDATA.Tools;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using System.Text.Json;

public class CheckActionPermissionAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;

        // اگر کاربر احراز هویت نشده باشد
        if (!user.Identity.IsAuthenticated)
        {
            context.Result = new JsonResult(new
            {
                success = false,
                message = "ابتدا وارد حساب کاربری خود شوید."
            })
            { StatusCode = StatusCodes.Status401Unauthorized };
            return;
        }

        // گرفتن کلید دسترسی (controller.action)
        var controller = context.RouteData.Values["controller"]?.ToString()?.ToLower();
        var action = context.RouteData.Values["action"]?.ToString()?.ToLower();
        var key = $"{controller}/{action}";

        // خواندن دسترسی‌ها از Claim
        var permissionsJson = user.FindFirst("PermissionsJson")?.Value;

        if (string.IsNullOrWhiteSpace(permissionsJson))
        {
            context.Result = new JsonResult(new
            {
                success = false,
                message = "مجوزهای دسترسی یافت نشد. لطفاً دوباره وارد شوید."
            })
            { StatusCode = StatusCodes.Status403Forbidden };
            return;
        }

        var permissions = JsonSerializer.Deserialize<List<string>>(permissionsJson.ToUnHash());

        if (!permissions.Contains(key))
        {
            context.Result = new JsonResult(new
            {
                success = false,
                message = "⛔ شما مجاز به دسترسی به این بخش نیستید."
            })
            { StatusCode = StatusCodes.Status403Forbidden };
        }
    }
}
