using MVP_Core.Data.Models;
using MVP_Core.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace MVP_Core.Services.Admin
{
    /// <summary>
    /// Configurable alert triggers for master admin notifications.
    /// </summary>
    public class SmartAdminAlertsService
    {
        private readonly ApplicationDbContext _db;
        public SmartAdminAlertsService(ApplicationDbContext db)
        {
            _db = db;
        }

        // Log an alert to AdminAlertLog
        public async Task LogAlertAsync(string alertType, string message, string severity)
        {
            var alert = new AdminAlertLog
            {
                Timestamp = DateTime.UtcNow,
                AlertType = alertType,
                Message = message,
                Severity = severity,
                IsResolved = false
            };
            _db.AdminAlertLogs.Add(alert);
            await _db.SaveChangesAsync();
        }

        // Acknowledge or mute an alert
        public async Task<bool> AcknowledgeAlertAsync(int alertId, string adminUserId, string actionTaken)
        {
            var alert = _db.AdminAlertLogs.FirstOrDefault(a => a.Id == alertId);
            if (alert == null) return false;
            // Optionally mark as resolved if acknowledged
            if (actionTaken == "Acknowledge") alert.IsResolved = true;
            _db.AdminAlertAcknowledgeLogs.Add(new AdminAlertAcknowledgeLog
            {
                AlertId = alertId,
                AdminUserId = adminUserId,
                ActionTaken = actionTaken,
                Timestamp = DateTime.UtcNow
            });
            await _db.SaveChangesAsync();
            return true;
        }

        // Get unresolved alerts, suppress acknowledged ones
        public async Task<List<AdminAlertLog>> GetActiveAlertsAsync(string adminUserId)
        {
            var acknowledgedIds = _db.AdminAlertAcknowledgeLogs
                .Where(x => x.AdminUserId == adminUserId && x.ActionTaken == "Acknowledge")
                .Select(x => x.AlertId)
                .ToList();
            var alerts = _db.AdminAlertLogs
                .Where(a => !a.IsResolved && !acknowledgedIds.Contains(a.Id))
                .OrderByDescending(a => a.Timestamp)
                .Take(10)
                .ToList();
            return await Task.FromResult(alerts);
        }

        // Get unresolved alerts (for dashboard)
        public async Task<List<string>> TriggerAlertsAsync()
        {
            var alerts = await Task.FromResult(_db.AdminAlertLogs
                .Where(a => !a.IsResolved)
                .OrderByDescending(a => a.Timestamp)
                .Take(10)
                .Select(a => $"[{a.Severity}] {a.Message}")
                .ToList());
            return alerts.Count > 0 ? alerts : new List<string> { "No critical alerts.", "System stable." };
        }

        // Sprint_91_11H: Placeholder for smart alert generation logic
        public void Alert() { }

        // Sprint_91_11I: Placeholder for smart alert generation logic
        public void Alert() { }

        // Sprint_91_11J: Placeholder for smart alert generation logic
        public void GenerateAlert() { }
    }
}
