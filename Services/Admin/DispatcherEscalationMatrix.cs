using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Data;
using Data.Models;

namespace Services.Admin
{
    public class DispatcherEscalationMatrix
    {
        private readonly ApplicationDbContext _db;
        public DispatcherEscalationMatrix(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<EscalationEvent>> CheckAndEscalate()
        {
            var sevenDaysAgo = DateTime.UtcNow.AddDays(-7);
            var logs = _db.LateClockInLogs
                .Where(l => l.Date >= sevenDaysAgo && (l.Severity == "Moderate" || l.Severity == "Severe"))
                .ToList();
            var grouped = logs.GroupBy(l => l.TechnicianId)
                .Where(g => g.Count() >= 3);
            var escalated = new List<EscalationEvent>();
            foreach (var group in grouped)
            {
                var techId = group.Key;
                if (!_db.EscalationEvents.Any(e => e.TechnicianId == techId && e.IncidentDate >= sevenDaysAgo))
                {
                    var evt = new EscalationEvent
                    {
                        TechnicianId = techId,
                        DispatcherId = 0, // Fill as needed
                        IncidentDate = DateTime.UtcNow,
                        Reason = $"{group.Count()} late clock-ins in 7 days",
                        EscalatedTo = "HR/Management",
                        Status = "Open"
                    };
                    _db.EscalationEvents.Add(evt);
                    escalated.Add(evt);
                }
            }
            await _db.SaveChangesAsync();
            return escalated;
        }

        public int GetPendingEscalationCount()
        {
            var sevenDaysAgo = DateTime.UtcNow.AddDays(-7);
            return _db.EscalationEvents.Count(e => e.Status == "Open" && e.IncidentDate >= sevenDaysAgo);
        }
    }
}
