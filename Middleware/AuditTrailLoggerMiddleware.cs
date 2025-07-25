// Sprint 32.2 - Security + Audit Harden
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using MVP_Core.Services;

namespace MVP_Core.Middleware
{
    public class AuditTrailLoggerMiddleware
    {
        private readonly RequestDelegate _next;
        public AuditTrailLoggerMiddleware(RequestDelegate next) => _next = next;
        public async Task InvokeAsync(HttpContext context, IAuditTrailLogger auditLogger)
        {
            // Only log for authenticated users and [Authorize] endpoints
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var userId = context.User.Identity.Name ?? "unknown";
                var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                var path = context.Request.Path.ToString();
                // Log only for protected/admin/api/technician endpoints
                if (path.StartsWith("/Admin") || path.StartsWith("/api/tech") || path.StartsWith("/api/admin"))
                {
                    await auditLogger.LogAsync(userId, $"Access {path}", ip);
                }
            }
            await _next(context);
        }
    }
}
