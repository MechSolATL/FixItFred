using MVP_Core.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace MVP_Core.Services.Admin
{
    public class SyncIncentiveEngine
    {
        private readonly ApplicationDbContext _db;
        public SyncIncentiveEngine(ApplicationDbContext db)
        {
            _db = db;
        }

        public List<TechnicianSyncScore> CalculateAllSyncScores()
        {
            var techs = _db.Technicians.ToList();
            var mediaLogs = _db.MediaSyncLogs.ToList();
            var zoneHeatmaps = _db.OfflineZoneHeatmaps.ToList();
            var scores = new List<TechnicianSyncScore>();
            foreach (var tech in techs)
            {
                var logs = mediaLogs.Where(m => m.TechnicianId == tech.Id).OrderByDescending(m => m.Timestamp).ToList();
                int total = logs.Count;
                int success = logs.Count(m => m.IsSuccess);
                double successRate = total > 0 ? (double)success / total : 0;
                // Streak logic
                int streak = 0;
                for (int i = 0; i < Math.Min(7, logs.Count); i++)
                {
                    if (logs[i].IsSuccess) streak++;
                    else break;
                }
                // Penalty logic
                int penaltyCount = logs.Take(7).Count(m => !m.IsSuccess);
                // Weight by high-failure zones
                var techZones = zoneHeatmaps.Where(z => z.TechnicianId == tech.Id);
                int highRiskZones = techZones.Count(z => z.FailureCount >= 3);
                double weightedRate = successRate;
                if (highRiskZones > 0)
                {
                    weightedRate += 0.05 * highRiskZones; // +5% per high-risk zone
                }
                weightedRate = Math.Min(weightedRate, 1.0);
                SyncRankLevel rank;
                double multiplier;
                if (weightedRate >= 0.90) { rank = SyncRankLevel.Platinum; multiplier = 2.0; }
                else if (weightedRate >= 0.75) { rank = SyncRankLevel.Gold; multiplier = 1.5; }
                else if (weightedRate >= 0.50) { rank = SyncRankLevel.Silver; multiplier = 1.2; }
                else { rank = SyncRankLevel.Bronze; multiplier = 1.0; }
                // Streak accelerator
                if (streak >= 5) multiplier += 0.5;
                // Bonus cooldown
                var score = _db.TechnicianSyncScores.FirstOrDefault(s => s.TechnicianId == tech.Id);
                DateTime? lastBonus = score?.LastBonusAwarded;
                DateTime? cooldownUntil = score?.CooldownUntil;
                bool bonusEligible = true;
                if (cooldownUntil.HasValue && DateTime.UtcNow < cooldownUntil.Value)
                {
                    bonusEligible = false;
                }
                // Auto-upgrade
                bool autoPromoted = false;
                if (weightedRate >= 0.98 && streak >= 5 && penaltyCount == 0)
                {
                    if (score != null && score.SyncRankLevel != SyncRankLevel.Platinum)
                    {
                        var previousRank = score.SyncRankLevel;
                        score.SyncRankLevel = SyncRankLevel.Platinum;
                        score.LastUpdated = DateTime.UtcNow;
                        score.BonusMultiplier = multiplier;
                        _db.SyncRankOverrideLogs.Add(new SyncRankOverrideLog
                        {
                            TechnicianId = tech.Id,
                            PreviousRank = previousRank,
                            NewRank = SyncRankLevel.Platinum,
                            Reason = "Auto upgrade: flawless streak and high sync %",
                            ModifiedBy = "Auto",
                            Timestamp = DateTime.UtcNow
                        });
                        _db.SaveChanges();
                        autoPromoted = true;
                    }
                }
                scores.Add(new TechnicianSyncScore
                {
                    TechnicianId = tech.Id,
                    SyncSuccessRate = Math.Round(weightedRate * 100, 1),
                    LastUpdated = DateTime.UtcNow,
                    SyncRankLevel = rank,
                    BonusMultiplier = multiplier,
                    LastBonusAwarded = lastBonus,
                    CooldownUntil = cooldownUntil,
                    // Custom fields for UI
                    BonusEligible = bonusEligible,
                    StreakLength = streak,
                    CooldownRemaining = cooldownUntil.HasValue ? (cooldownUntil.Value - DateTime.UtcNow).TotalDays : 0,
                    AutoPromoted = autoPromoted,
                    RecentPenaltyCount = penaltyCount
                });
            }
            return scores;
        }
        public void OverrideSyncRank(int technicianId, SyncRankLevel newRank, string reason, string modifiedBy)
        {
            var score = _db.TechnicianSyncScores.FirstOrDefault(s => s.TechnicianId == technicianId);
            if (score != null)
            {
                var previousRank = score.SyncRankLevel;
                score.SyncRankLevel = newRank;
                score.LastUpdated = DateTime.UtcNow;
                score.BonusMultiplier = newRank switch
                {
                    SyncRankLevel.Platinum => 2.0,
                    SyncRankLevel.Gold => 1.5,
                    SyncRankLevel.Silver => 1.2,
                    _ => 1.0
                };
                _db.SyncRankOverrideLogs.Add(new SyncRankOverrideLog
                {
                    TechnicianId = technicianId,
                    PreviousRank = previousRank,
                    NewRank = newRank,
                    Reason = reason,
                    ModifiedBy = modifiedBy,
                    Timestamp = DateTime.UtcNow
                });
                _db.SaveChanges();
            }
        }
        public List<SyncRankOverrideLog> GetOverrideLog(int technicianId)
        {
            return _db.SyncRankOverrideLogs.Where(l => l.TechnicianId == technicianId).OrderByDescending(l => l.Timestamp).ToList();
        }
        public string GenerateCsvReport()
        {
            var scores = CalculateAllSyncScores();
            var db = _db;
            var techs = db.Technicians.ToList();
            var sb = new StringBuilder();
            sb.AppendLine("TechnicianId,Name,SyncSuccessRate,SyncRankLevel,BonusMultiplier,LastUpdated");
            foreach (var score in scores)
            {
                var tech = techs.FirstOrDefault(t => t.Id == score.TechnicianId);
                sb.AppendLine($"{score.TechnicianId},{tech?.Name},{score.SyncSuccessRate},{score.SyncRankLevel},{score.BonusMultiplier},{score.LastUpdated:yyyy-MM-dd HH:mm:ss}");
            }
            return sb.ToString();
        }
        public int GetConsecutiveFailedDays(int technicianId)
        {
            var logs = _db.MediaSyncLogs.Where(m => m.TechnicianId == technicianId).OrderByDescending(m => m.Timestamp).Take(7).ToList();
            int count = 0;
            DateTime? lastDay = null;
            foreach (var log in logs)
            {
                if (!log.IsSuccess)
                {
                    if (lastDay == null || log.Timestamp.Date == lastDay.Value.Date.AddDays(-1))
                    {
                        count++;
                        lastDay = log.Timestamp.Date;
                    }
                }
                else break;
            }
            return count;
        }
    }
}
