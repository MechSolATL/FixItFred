using System.Threading.Tasks;
using MVP_Core.Data;
using System;
using System.Linq;

namespace Services.Admin
{
    /// <summary>
    /// Sends daily admin digest summary (alerts, snapshot status, storage usage).
    /// </summary>
    public class AdminDigestMailerService
    {
        private readonly ApplicationDbContext _db;
        public AdminDigestMailerService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<string> SendDigestAsync()
        {
            // Mock: Compose summary
            var alertCount = _db.AdminAlertLogs.Count(a => !a.IsResolved);
            var snapshotCount = _db.SystemSnapshotLogs.Count();
            var storageUsage = _db.StorageGrowthSnapshots.OrderByDescending(s => s.Timestamp).FirstOrDefault()?.UsageMB ?? 0;
            var summary = $"Daily Digest: {alertCount} active alerts, {snapshotCount} snapshots, {storageUsage}MB used.";
            // Simulate sending email
            await Task.Delay(100);
            return summary;
        }
    }
}
