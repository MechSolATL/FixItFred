using System;
using System.Collections.Generic;
using System.Linq;
using MVP_Core.Data;
using MVP_Core.Data.Models;
using MVP_Core.Data.Enums;

namespace MVP_Core.Services.Compliance
{
    public class ComplianceReminderService
    {
        private readonly ApplicationDbContext _db;
        public ComplianceReminderService(ApplicationDbContext db)
        {
            _db = db;
        }
        public List<ComplianceAlertLog> GetActiveReminders()
        {
            var now = DateTime.UtcNow;
            return _db.ComplianceAlertLogs
                .Where(a => !a.IsAcknowledged && (a.ExpiresAt == null || a.ExpiresAt > now))
                .OrderBy(a => a.ExpiresAt)
                .ToList();
        }
        public void AcknowledgeAlert(int alertId, string user)
        {
            var alert = _db.ComplianceAlertLogs.FirstOrDefault(a => a.Id == alertId);
            if (alert != null)
            {
                alert.IsAcknowledged = true;
                alert.AcknowledgedBy = user;
                alert.AcknowledgedAt = DateTime.UtcNow;
                _db.SaveChanges();
            }
        }
    }
}
