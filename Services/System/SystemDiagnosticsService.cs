using MVP_Core.Data.Models;
using MVP_Core.Data;
using Microsoft.Extensions.Logging;
using Services.Admin;
using MVP_Core.DTOs.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;

namespace MVP_Core.Services.System
{
    public class SystemDiagnosticsService
    {
        private readonly ApplicationDbContext _db;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<SystemDiagnosticsService> _logger;
        private readonly AutoRepairEngine _autoRepairEngine;
        private readonly SmartAdminAlertsService _alertsService;

        public SystemDiagnosticsService(ApplicationDbContext db, IServiceProvider serviceProvider, ILogger<SystemDiagnosticsService> logger, AutoRepairEngine autoRepairEngine, SmartAdminAlertsService alertsService)
        {
            _db = db;
            _serviceProvider = serviceProvider;
            _logger = logger;
            _autoRepairEngine = autoRepairEngine;
            _alertsService = alertsService;
        }

        public async Task<DiagnosticReportDTO> RunDiagnosticsAsync(bool triggerSnapshot = false)
        {
            var warnings = new List<string>();
            int errorCount = 0;
            string status = "OK";

            // 1. Razor page model binding mismatches (stub: add real checks as needed)
            warnings.Add("Model binding mismatch check not implemented.");

            // 2. Missing EF DbSet<> for existing tables
            warnings.Add("Missing DbSet<> check not fully implemented.");

            // 3. Unresponsive services in DI container
            warnings.Add("DI container health check not implemented.");

            // 4. Last 10 log entries with errors/exceptions
            var lastLogs = _db.SystemDiagnosticLogs.OrderByDescending(l => l.Timestamp).Take(10).ToList();
            errorCount = lastLogs.Count(l => l.StatusLevel == DiagnosticStatusLevel.Error);
            if (errorCount > 0) status = "Error";
            else if (warnings.Count > 0) status = "Warning";

            // 5. Log alerts for warnings/errors
            if (errorCount > 0 || warnings.Count > 0)
            {
                string alertLevel = errorCount > 0 ? "Critical" : "Warning";
                string alertMsg = $"Diagnostics: {errorCount} errors, {warnings.Count} warnings.";
                await _alertsService.LogAlertAsync("SystemDiagnosticsService", alertMsg, alertLevel);
            }

            // 6. Trigger snapshot if requested
            if (triggerSnapshot)
            {
                // Create a unique hash for the snapshot
                string hash = GenerateSnapshotHash();
                var snapshot = new SystemSnapshotLog
                {
                    Timestamp = DateTime.UtcNow,
                    SnapshotType = "DiagnosticsAutoSnapshot",
                    Summary = $"Snapshot after diagnostics. Status: {status}",
                    DetailsJson = "{}",
                    CreatedBy = "SystemDiagnosticsService"
                };
                _db.SystemSnapshotLogs.Add(snapshot);
                await _db.SaveChangesAsync();
            }

            return new DiagnosticReportDTO
            {
                Status = status,
                ErrorCount = errorCount,
                Warnings = warnings
            };
        }

        // Helper to generate a unique hash for snapshot
        private string GenerateSnapshotHash()
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(DateTime.UtcNow.ToString("o") + Guid.NewGuid()));
            return BitConverter.ToString(bytes).Replace("-", "");
        }
    }
}
