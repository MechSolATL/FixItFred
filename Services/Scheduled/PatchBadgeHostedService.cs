using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Data.Models.PatchAnalytics;
using Data;

namespace Services.Scheduled
{
    // Sprint 91.22.4 - PatchBadgeHostedService
    public class PatchBadgeHostedService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<PatchBadgeHostedService> _logger;
        public PatchBadgeHostedService(IServiceProvider serviceProvider, ILogger<PatchBadgeHostedService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.Now;
                // Calculate next Sunday 11:59 PM
                int daysUntilSunday = ((int)DayOfWeek.Sunday - (int)now.DayOfWeek + 7) % 7;
                var nextRun = now.Date.AddDays(daysUntilSunday).AddHours(23).AddMinutes(59);
                if (nextRun <= now)
                    nextRun = nextRun.AddDays(7);
                var delay = nextRun - now;
                _logger.LogInformation($"PatchBadgeHostedService sleeping for {delay.TotalMinutes} minutes until {nextRun}.");
                await Task.Delay(delay, stoppingToken);
                if (stoppingToken.IsCancellationRequested) break;
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var badgeService = scope.ServiceProvider.GetRequiredService<BadgeAssignmentService>();
                        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                        // Safeguard: skip if badges already assigned this week
                        var weekAgo = DateTime.UtcNow.AddDays(-7);
                        bool alreadyAssigned = db.Set<TechnicianBadge>().Any(b => b.EarnedOn >= weekAgo);
                        if (!alreadyAssigned)
                        {
                            await badgeService.AssignWeeklyBadgesAsync();
                            db.Add(new PatchSystemLog { Timestamp = DateTime.UtcNow, Message = "PatchBadgeHostedService: Badges assigned." });
                            await db.SaveChangesAsync();
                            _logger.LogInformation("PatchBadgeHostedService: Badges assigned.");
                        }
                        else
                        {
                            _logger.LogInformation("PatchBadgeHostedService: Badges already assigned this week. Skipping.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "PatchBadgeHostedService: Error assigning badges.");
                    // Future: Email admins if badge assignment fails
                }
            }
        }
    }
}
