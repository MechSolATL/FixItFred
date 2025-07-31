using MVP_Core.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using Data;

namespace Services.Admin
{
    public class TechnicianKarmaService
    {
        private readonly ApplicationDbContext _db;
        public TechnicianKarmaService(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<int> CalculateKarmaAsync(int technicianId)
        {
            // Composite: trust, morale, warnings, flags, response, audit
            var trust = await _db.TechnicianTrustLogs.Where(x => x.TechnicianId == technicianId).OrderByDescending(x => x.RecordedAt).Select(x => (int?)x.TrustScore).FirstOrDefaultAsync() ?? 50;
            var morale = await _db.TechnicianMoraleLogs.Where(x => x.TechnicianId == technicianId).OrderByDescending(x => x.Timestamp).Select(x => (int?)x.MoraleScore).FirstOrDefaultAsync() ?? 50;
            var warnings = await _db.TechnicianWarningLogs.CountAsync(x => x.TechnicianId == technicianId && x.Timestamp > DateTime.UtcNow.AddDays(-30));
            var flags = await _db.TechnicianEscalationLogs.CountAsync(x => x.TechnicianId == technicianId && x.CreatedAt > DateTime.UtcNow.AddDays(-30));
            var response = await _db.TechnicianResponseLogs.CountAsync(x => x.TechnicianId == technicianId && x.Timestamp > DateTime.UtcNow.AddDays(-30));
            var audit = await _db.TechnicianAuditLogs.CountAsync(x => x.TechnicianId == technicianId && x.Timestamp > DateTime.UtcNow.AddDays(-30));
            // Example weights
            int score = trust + morale - warnings * 5 - flags * 3 + response * 2 - audit * 1;
            score = Math.Max(0, Math.Min(100, score));
            return score;
        }
        public async Task RecalculateAllAsync()
        {
            var techs = await _db.Technicians.Select(t => t.Id).ToListAsync();
            foreach (var techId in techs)
            {
                var score = await CalculateKarmaAsync(techId);
                var prev = await _db.TechnicianKarmaLogs.Where(x => x.TechnicianId == techId).OrderByDescending(x => x.Timestamp).FirstOrDefaultAsync();
                string trend = prev == null ? "Stable" : score > prev.KarmaScore ? "Up" : score < prev.KarmaScore ? "Down" : "Stable";
                _db.TechnicianKarmaLogs.Add(new TechnicianKarmaLog
                {
                    TechnicianId = techId,
                    KarmaScore = score,
                    Trend = trend,
                    KarmaCategory = "Composite",
                    ChangeSource = "System",
                    Reason = "Auto recalculation",
                    Timestamp = DateTime.UtcNow
                });
            }
            await _db.SaveChangesAsync();
        }
        public async Task ApplyManualAdjustment(int technicianId, int newScore, string reason, string admin)
        {
            var prev = await _db.TechnicianKarmaLogs.Where(x => x.TechnicianId == technicianId).OrderByDescending(x => x.Timestamp).FirstOrDefaultAsync();
            string trend = prev == null ? "Stable" : newScore > prev.KarmaScore ? "Up" : newScore < prev.KarmaScore ? "Down" : "Stable";
            _db.TechnicianKarmaLogs.Add(new TechnicianKarmaLog
            {
                TechnicianId = technicianId,
                KarmaScore = newScore,
                Trend = trend,
                KarmaCategory = "Manual",
                ChangeSource = admin,
                Reason = reason,
                Timestamp = DateTime.UtcNow
            });
            await _db.SaveChangesAsync();
        }
        public async Task<List<TechnicianKarmaLog>> GetKarmaHistoryAsync(int technicianId)
        {
            return await _db.TechnicianKarmaLogs.Where(x => x.TechnicianId == technicianId).OrderByDescending(x => x.Timestamp).ToListAsync();
        }
    }
}
