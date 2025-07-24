using System.Text.RegularExpressions;

namespace MVP_Core.Middleware
{
    public class PageVisitLoggerMiddleware
    {
        private readonly RequestDelegate _next;

        public PageVisitLoggerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ApplicationDbContext dbContext)
        {
            // Let request proceed first
            await _next(context);

            try
            {
                string path = context.Request.Path.Value ?? string.Empty;

                // Skip logging for admin, static, or internal requests
                if (path.StartsWith("/Admin", StringComparison.OrdinalIgnoreCase) ||
                    path.StartsWith("/api", StringComparison.OrdinalIgnoreCase) ||
                    path.StartsWith("/_framework", StringComparison.OrdinalIgnoreCase) ||
                    path.StartsWith("/css", StringComparison.OrdinalIgnoreCase) ||
                    path.StartsWith("/js", StringComparison.OrdinalIgnoreCase) ||
                    path.StartsWith("/stream", StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }

                string userAgent = context.Request.Headers["User-Agent"].ToString();

                // Only log real users based on browser patterns
                bool isRealUser = Regex.IsMatch(userAgent, "Chrome|Safari|Edge|Firefox", RegexOptions.IgnoreCase);

                PageVisitLog log = new()
                {
                    PageUrl = context.Request.Path.HasValue ? context.Request.Path.Value : "Unknown",
                    Referrer = context.Request.Headers["Referer"].FirstOrDefault() ?? "Direct/Unknown",
                    UserAgent = !string.IsNullOrWhiteSpace(userAgent) ? userAgent : "Unknown",
                    IpAddress = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                    IsRealUser = isRealUser,
                    ResponseStatusCode = context.Response.StatusCode,
                    VisitTimestamp = DateTime.UtcNow
                };

                _ = dbContext.PageVisitLogs.Add(log);
                _ = dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Logging middleware error: {ex.Message}");
                // Optional: Log to file or external logger here
            }
        }
    }
}
