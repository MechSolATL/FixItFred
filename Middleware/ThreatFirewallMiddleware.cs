using MVP_Core.Data;
using MVP_Core.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace MVP_Core.Middleware
{
    public class ThreatFirewallMiddleware
    {
        private readonly RequestDelegate _next;

        public ThreatFirewallMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, ApplicationDbContext dbContext)
        {
            var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            var userAgent = context.Request.Headers["User-Agent"].ToString() ?? "Unknown";

            // Skip localhost during dev
            if (ipAddress.StartsWith("127.") || ipAddress == "::1")
            {
                await _next(context);
                return;
            }

            var threat = await dbContext.ThreatBlocks
                .FirstOrDefaultAsync(t => t.IpAddress == ipAddress && t.UserAgent == userAgent);

            if (threat != null)
            {
                if (threat.IsPermanentlyBlocked)
                {
                    context.Response.Redirect("/Blocked");
                    return;
                }

                if (threat.BanLiftTime.HasValue && threat.BanLiftTime.Value > DateTime.UtcNow)
                {
                    context.Response.Redirect("/Blocked");
                    return;
                }
            }

            // After pipeline processes
            await _next(context);

            // If response is suspicious (e.g., 404, 403, 500), count it as a "strike"
            if (context.Response.StatusCode >= 400)
            {
                await RegisterStrike(dbContext, ipAddress, userAgent);
            }
        }

        private async Task RegisterStrike(ApplicationDbContext dbContext, string ipAddress, string userAgent)
        {
            var now = DateTime.UtcNow;
            var threat = await dbContext.ThreatBlocks
                .FirstOrDefaultAsync(t => t.IpAddress == ipAddress && t.UserAgent == userAgent);

            if (threat == null)
            {
                threat = new ThreatBlock
                {
                    IpAddress = ipAddress,
                    UserAgent = userAgent,
                    StrikeCount = 1,
                    FirstDetectedAt = now,
                    LastDetectedAt = now,
                    IsPermanentlyBlocked = false,
                    BanLiftTime = now.AddHours(1) // First strike penalty
                };
                dbContext.ThreatBlocks.Add(threat);
            }
            else
            {
                threat.StrikeCount++;
                threat.LastDetectedAt = now;

                if (threat.StrikeCount == 2)
                    threat.BanLiftTime = now.AddHours(12);
                else if (threat.StrikeCount == 3)
                    threat.BanLiftTime = now.AddHours(24);
                else if (threat.StrikeCount >= 4)
                    threat.IsPermanentlyBlocked = true;
            }

            await dbContext.SaveChangesAsync();
        }
    }
}
