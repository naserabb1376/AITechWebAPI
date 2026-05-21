namespace AITechWebAPI.Tools
{
    public sealed class ApiErrorResponse
    {
        public int StatusCode { get; init; }
        public string Title { get; init; } = "خطایی رخ داده است";
        public string Message { get; init; } = "لطفا کمی بعد دوباره تلاش کنید.";
        public string TraceId { get; init; } = string.Empty;
        public string Path { get; init; } = string.Empty;
        public string Method { get; init; } = string.Empty;
        public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;
        public string? Detail { get; init; }
        public IDictionary<string, string[]>? Errors { get; init; }
    }
}
