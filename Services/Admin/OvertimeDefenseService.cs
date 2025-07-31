using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Data;
using Data.Models;

namespace Services.Admin
{
    public class OvertimeDefenseService
    {
        private readonly ApplicationDbContext _db;
        public OvertimeDefenseService(ApplicationDbContext db)
        {
            _db = db;
        }
        // Detects overworked sessions
        public List<OvertimeLockoutLog> DetectOverworkedSessions(int technicianId)
        {
            var logs = _db.OvertimeLockoutLogs.Where(l => l.TechnicianId == technicianId).OrderByDescending(l => l.EventTime).ToList();
            return logs;
        }
        // Issues lockout recommendations
        public OvertimeLockoutLog IssueLockoutRecommendation(int technicianId, string systemDecision)
        {
            var log = new OvertimeLockoutLog
            {
                TechnicianId = technicianId,
                EventType = "LockoutRecommendation",
                SystemDecision = systemDecision,
                EventTime = DateTime.UtcNow
            };
            _db.OvertimeLockoutLogs.Add(log);
            _db.SaveChanges();
            return log;
        }
        // Supports override logic
        public void OverrideLockout(int logId, string reason, string approver)
        {
            var log = _db.OvertimeLockoutLogs.FirstOrDefault(l => l.Id == logId);
            if (log != null)
            {
                log.IsOverride = true;
                log.OverrideReason = reason;
                log.Approver = approver;
                _db.SaveChanges();
            }
        }
        // Logs all system and manual actions
        public List<OvertimeLockoutLog> GetAllLogs() => _db.OvertimeLockoutLogs.OrderByDescending(l => l.EventTime).ToList();
    }
}
