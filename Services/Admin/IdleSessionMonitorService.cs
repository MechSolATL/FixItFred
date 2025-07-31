using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Data.Models;
using Data;

namespace Services.Admin
{
    public class IdleSessionMonitorService
    {
        private readonly ApplicationDbContext _db;
        public IdleSessionMonitorService(ApplicationDbContext db)
        {
            _db = db;
        }
        // Monitors technician activity for idle sessions over 30 minutes
        public List<IdleSessionMonitorLog> GetIdleSessions(int technicianId)
        {
            return _db.IdleSessionMonitorLogs.Where(l => l.TechnicianId == technicianId && l.IdleMinutes > 30).OrderByDescending(l => l.IdleStartTime).ToList();
        }
        // Logs auto-timeout or manual override
        public IdleSessionMonitorLog LogIdleTimeout(int technicianId, int idleMinutes, string systemDecision)
        {
            var log = new IdleSessionMonitorLog
            {
                TechnicianId = technicianId,
                IdleStartTime = DateTime.UtcNow.AddMinutes(-idleMinutes),
                IdleEndTime = DateTime.UtcNow,
                IdleMinutes = idleMinutes,
                SystemDecision = systemDecision,
                ClockOutTime = DateTime.UtcNow
            };
            _db.IdleSessionMonitorLogs.Add(log);
            _db.SaveChanges();
            return log;
        }
        // Triggers warnings or logs manager response time
        public void OverrideIdleTimeout(int logId, string reason, string approver)
        {
            var log = _db.IdleSessionMonitorLogs.FirstOrDefault(l => l.Id == logId);
            if (log != null)
            {
                log.IsOverride = true;
                log.OverrideReason = reason;
                log.Approver = approver;
                _db.SaveChanges();
            }
        }
        public List<IdleSessionMonitorLog> GetAllLogs() => _db.IdleSessionMonitorLogs.OrderByDescending(l => l.IdleStartTime).ToList();
    }
}
