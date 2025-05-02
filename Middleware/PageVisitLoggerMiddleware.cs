using MVP_Core.Data;
using MVP_Core.Data.Models;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

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
            // Always allow pipeline to process request first
            await _next(context);

            // Skip logging for /Admin or /api/ or /static/ requests
            var path = context.Request.Path.Value ?? string.Empty;
            if (path.StartsWith("/Admin", StringComparison.OrdinalIgnoreCase) ||
                path.StartsWith("/api", StringComparison.OrdinalIgnoreCase) ||
                path.StartsWith("/_framework", StringComparison.OrdinalIgnoreCase) ||
                path.StartsWith("/css", StringComparison.OrdinalIgnoreCase) ||
                path.StartsWith("/js", StringComparison.OrdinalIgnoreCase) ||
                path.StartsWith("/stream", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var userAgent = context.Request.Headers["User-Agent"].ToString();

            // Smart Real User Detector: ignore bots/tools
            bool isRealUser = Regex.IsMatch(userAgent, "Chrome|Safari|Edge|Firefox", RegexOptions.IgnoreCase);

            var log = new PageVisitLog
            {
                PageUrl = context.Request.Path.HasValue ? context.Request.Path.Value : "Unknown",
                Referrer = context.Request.Headers["Referer"].FirstOrDefault() ?? "Direct/Unknown",
                UserAgent = userAgent ?? "Unknown",
                IpAddress = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                IsRealUser = isRealUser,
                ResponseStatusCode = context.Response.StatusCode,
                VisitTimestamp = DateTime.UtcNow
            };

            try
            {
                dbContext.PageVisitLogs.Add(log);
                await dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Log if needed, but don't crash the request flow
                // e.g., logger.LogError(ex, "Failed to save page visit log.");
            }
        }
    }
}
