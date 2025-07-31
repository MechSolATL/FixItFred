using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Data;

namespace Services.Analytics
{
    // Sprint 86.5 — Background cleanup for old user action logs
    public class UserActionLogCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        public UserActionLogCleanupService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var db = (ApplicationDbContext)scope.ServiceProvider.GetService(typeof(ApplicationDbContext));
                    var cutoff = DateTime.UtcNow.AddDays(-30);
                    var oldLogs = await db.UserActionLogs.Where(l => l.Timestamp < cutoff).ToListAsync(stoppingToken);
                    if (oldLogs.Count > 0)
                    {
                        db.UserActionLogs.RemoveRange(oldLogs);
                        await db.SaveChangesAsync(stoppingToken);
                    }
                }
                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }
    }
}
