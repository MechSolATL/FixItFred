using Data;
using Data.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Admin
{
    public class LateClockInDetectorService
    {
        private readonly ApplicationDbContext _db;
        public LateClockInDetectorService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task AnalyzeAndLogClockIn(int technicianId, DateTime scheduledStart, DateTime actualStart)
        {
            int delay = (int)(actualStart - scheduledStart).TotalMinutes;
            if (delay < 5) return; // Not late
            string severity = delay switch
            {
                >= 30 => "Severe",
                >= 15 => "Moderate",
                >= 5 => "Warning",
                _ => "OnTime"
            };
            var log = new LateClockInLog
            {
                TechnicianId = technicianId,
                ScheduledStart = scheduledStart,
                ActualStart = actualStart,
                DelayMinutes = delay,
                Date = actualStart.Date,
                Severity = severity
            };
            _db.LateClockInLogs.Add(log);
            await _db.SaveChangesAsync();
        }
    }
}
