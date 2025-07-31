using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Data;
using Models.Admin;

namespace Services.Admin
{
    // Sprint 84.9 — Drop Alert Logic + TrustScore Delta Detection
    // Sprint 85.3 — Triggered Coaching Suggestions
    public class TrustScoreDropAlertService
    {
        private readonly ApplicationDbContext _db;
        private readonly CoachingLogService _coachingLogService;
        public TrustScoreDropAlertService(ApplicationDbContext db, CoachingLogService coachingLogService)
        {
            _db = db;
            _coachingLogService = coachingLogService;
        }

        // Checks all technicians for HeatScore drops and logs alerts
        public async Task<int> CheckAndLogDropsAsync()
        {
            var count = 0;
            var techs = await _db.Technicians.ToListAsync();
            foreach (var tech in techs)
            {
                int? lastKnown = tech.LastKnownHeatScore;
                int current = tech.HeatScore;
                int drop = lastKnown.HasValue ? lastKnown.Value - current : 0;
                if (lastKnown.HasValue && drop >= 15)
                {
                    // Sprint 85.3 — Triggered Coaching Suggestions
                    bool triggerCoaching = false;
                    if (drop > 25)
                    {
                        var logs = _coachingLogService.GetLogsForTechnician(tech.Id);
                        var lastCoaching = logs.FirstOrDefault();
                        if (lastCoaching == null || lastCoaching.Timestamp < DateTime.UtcNow.AddDays(-21))
                        {
                            triggerCoaching = true;
                        }
                    }
                    _db.TechnicianAlertLogs.Add(new TechnicianAlertLog
                    {
                        TechnicianId = tech.Id,
                        PreviousScore = lastKnown.Value,
                        CurrentScore = current,
                        TriggeredAt = DateTime.UtcNow,
                        Acknowledged = false,
                        // Sprint 85.3 — Triggered Coaching Suggestions
                        TriggeredCoachingRecommended = triggerCoaching
                    });
                    count++;
                }
                // Always update LastKnownHeatScore
                tech.LastKnownHeatScore = current;
                _db.Technicians.Update(tech);
            }
            await _db.SaveChangesAsync();
            return count;
        }

        // Returns unacknowledged drop alerts
        public async Task<IQueryable<TechnicianAlertLog>> GetUnacknowledgedAlertsAsync()
        {
            return _db.TechnicianAlertLogs.Where(a => !a.Acknowledged);
        }
    }
}
