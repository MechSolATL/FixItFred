using MVP_Core.Data.Models;
using MVP_Core.Data;
using MVP_Core.Services.Admin;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Diagnostics
{
    /// <summary>
    /// Scans logs and flags possible root causes for failures.
    /// </summary>
    public class RootCauseCorrelationEngine
    {
        private readonly ApplicationDbContext _db;
        private readonly SmartAdminAlertsService _alertsService;
        public RootCauseCorrelationEngine(ApplicationDbContext db, SmartAdminAlertsService alertsService)
        {
            _db = db;
            _alertsService = alertsService;
        }

        public async Task<string?> CorrelateRootCausesAsync()
        {
            // Simulate summary
            string summary = "No root causes detected.";
            // Example: If a root cause is found, log an alert
            var recentError = _db.SystemDiagnosticLogs.OrderByDescending(l => l.Timestamp).FirstOrDefault(l => l.StatusLevel == DiagnosticStatusLevel.Error);
            if (recentError != null)
            {
                await _alertsService.LogAlertAsync("RootCauseCorrelationEngine", $"Root cause detected: {recentError.Summary}", "Critical");
                summary = $"Root cause detected: {recentError.Summary}";
            }
            return summary;
        }
    }
}
