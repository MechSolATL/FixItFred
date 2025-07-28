using MVP_Core.Data;
using MVP_Core.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MVP_Core.Services.Admin
{
    // Sprint 84.9 — Drop Alert Logic + TrustScore Delta Detection
    public class TrustScoreDropAlertService
    {
        private readonly ApplicationDbContext _db;
        public TrustScoreDropAlertService(ApplicationDbContext db)
        {
            _db = db;
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
                if (lastKnown.HasValue && (lastKnown.Value - current) >= 15)
                {
                    // Log alert
                    _db.TechnicianAlertLogs.Add(new TechnicianAlertLog
                    {
                        TechnicianId = tech.Id,
                        PreviousScore = lastKnown.Value,
                        CurrentScore = current,
                        TriggeredAt = DateTime.UtcNow,
                        Acknowledged = false
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
