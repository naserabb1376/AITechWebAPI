using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AITechWebAPI.Tools
{
    public sealed class ExceptionHandlingMiddleware : IMiddleware
    {
        private readonly IWebHostEnvironment _environment;

        public ExceptionHandlingMiddleware(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex, _environment);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception ex, IWebHostEnvironment environment)
        {
            if (context.Response.HasStarted)
                return;

            context.Response.ContentType = "application/json";

            HttpStatusCode statusCode = ex switch
            {
                UnauthorizedAccessException => HttpStatusCode.Unauthorized,
                KeyNotFoundException => HttpStatusCode.NotFound,
                ArgumentException => HttpStatusCode.BadRequest,
                BadHttpRequestException => HttpStatusCode.BadRequest,
                TaskCanceledException => HttpStatusCode.RequestTimeout,
                TimeoutException => HttpStatusCode.RequestTimeout,
                _ => HttpStatusCode.InternalServerError
            };

            context.Response.StatusCode = (int)statusCode;

            var payload = new ApiErrorResponse
            {
                StatusCode = context.Response.StatusCode,
                Title = GetTitle(statusCode),
                Message = GetPublicMessage(statusCode, ex, environment),
                Detail = environment.IsDevelopment() ? ex.ToString() : null,
                TraceId = context.TraceIdentifier,
                Path = context.Request.Path,
                Method = context.Request.Method,
                Timestamp = DateTimeOffset.UtcNow
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(payload, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            }));
        }

        private static string GetTitle(HttpStatusCode statusCode)
        {
            return statusCode switch
            {
                HttpStatusCode.BadRequest => "درخواست نامعتبر است",
                HttpStatusCode.Unauthorized => "نیاز به ورود",
                HttpStatusCode.Forbidden => "دسترسی مجاز نیست",
                HttpStatusCode.NotFound => "موردی پیدا نشد",
                HttpStatusCode.RequestTimeout => "زمان درخواست تمام شد",
                HttpStatusCode.Conflict => "تداخل در اطلاعات",
                HttpStatusCode.ServiceUnavailable => "سرویس در دسترس نیست",
                _ => "خطای سرور"
            };
        }

        private static string GetPublicMessage(HttpStatusCode statusCode, Exception ex, IWebHostEnvironment environment)
        {
            if (environment.IsDevelopment())
                return ex.Message;

            return statusCode switch
            {
                HttpStatusCode.BadRequest => "اطلاعات ارسال‌شده معتبر نیست.",
                HttpStatusCode.Unauthorized => "برای ادامه باید وارد حساب کاربری شوید.",
                HttpStatusCode.Forbidden => "حساب شما اجازه دسترسی به این بخش را ندارد.",
                HttpStatusCode.NotFound => "اطلاعات درخواستی پیدا نشد.",
                HttpStatusCode.RequestTimeout => "ارتباط با سرور بیش از حد طول کشید.",
                _ => "در پردازش درخواست مشکلی رخ داده است."
            };
        }
    }
}
