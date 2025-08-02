using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;
using Data;
// [Sprint1002_FixItFred] Added missing using for BackgroundService
using Microsoft.Extensions.Hosting;

namespace Services.Admin
{
    /// <summary>
    /// Scheduled maintenance engine for log compression, alert pruning, and snapshot cleanup.
    /// </summary>
    public class ScheduledMaintenanceEngine : BackgroundService
    {
        private readonly ApplicationDbContext _db;
        private readonly IServiceProvider _serviceProvider;
        public ScheduledMaintenanceEngine(ApplicationDbContext db, IServiceProvider serviceProvider)
        {
            _db = db;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(System.Threading.CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await RunScheduledMaintenanceAsync();
                await Task.Delay(TimeSpan.FromHours(24), stoppingToken); // Daily
            }
        }

        public async Task RunScheduledMaintenanceAsync()
        {
            // Compress old logs (older than 30 days)
            var oldLogs = _db.SystemDiagnosticLogs.Where(l => l.Timestamp < DateTime.UtcNow.AddDays(-30)).ToList();
            if (oldLogs.Any())
            {
                // Simulate compression (could archive to file, etc.)
                foreach (var log in oldLogs) log.Details = "[COMPRESSED]";
                await _db.SaveChangesAsync();
            }
            // Prune alert history (older than 60 days)
            var oldAlerts = _db.AdminAlertLogs.Where(a => a.Timestamp < DateTime.UtcNow.AddDays(-60)).ToList();
            if (oldAlerts.Any())
            {
                _db.AdminAlertLogs.RemoveRange(oldAlerts);
                await _db.SaveChangesAsync();
            }
            // Purge snapshot overflow (keep last 100)
            var snapshots = _db.SystemSnapshotLogs.OrderByDescending(s => s.Timestamp).Skip(100).ToList();
            if (snapshots.Any())
            {
                _db.SystemSnapshotLogs.RemoveRange(snapshots);
                await _db.SaveChangesAsync();
            }
        }
    }
}
