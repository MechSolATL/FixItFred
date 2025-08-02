using Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Data;

namespace Services.Admin
{
    public class TechnicianTrustEngineService
    {
        private readonly ApplicationDbContext _db;

        public TechnicianTrustEngineService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<decimal> CalculateTrustScoreAsync(int technicianId)
        {
            // Get recent logs
            var recentBehavior = await _db.TechnicianBehaviorLogs.Where(x => x.TechnicianId == technicianId && x.Timestamp > DateTime.UtcNow.AddDays(-30)).ToListAsync();
            var recentWarnings = await _db.TechnicianWarningLogs.Where(x => x.TechnicianId == technicianId && x.Timestamp > DateTime.UtcNow.AddDays(-30)).ToListAsync();
            var recentResponses = await _db.TechnicianResponseLogs.Where(x => x.TechnicianId == technicianId && x.Timestamp > DateTime.UtcNow.AddDays(-30)).ToListAsync();
            var flagCount = recentWarnings.Count + recentBehavior.Count;

            // Example weights (can be tuned)
            decimal behaviorWeight = 0.4m;
            decimal warningWeight = 0.3m;
            decimal responseWeight = 0.2m;
            decimal flagWeight = 0.1m;

            // Calculate scores
            decimal behaviorScore = 100 - recentBehavior.Count * 5;
            decimal warningScore = 100 - recentWarnings.Count * 10;
            decimal responseScore = recentResponses.Count > 0 ? 100 : 80;
            decimal flagScore = 100 - flagCount * 7;

            decimal trustScore = behaviorScore * behaviorWeight + warningScore * warningWeight + responseScore * responseWeight + flagScore * flagWeight;
            trustScore = Math.Max(0, Math.Min(100, trustScore));
            return trustScore;
        }

        public async Task LogTrustIndexAsync(int technicianId, decimal trustScore, decimal flagWeight)
        {
            var log = new TechnicianTrustLog
            {
                TechnicianId = technicianId,
                TrustScore = trustScore,
                FlagWeight = flagWeight,
                RecordedAt = DateTime.UtcNow
            };
            _db.TechnicianTrustLogs.Add(log);
            await _db.SaveChangesAsync();
        }
    }
}
