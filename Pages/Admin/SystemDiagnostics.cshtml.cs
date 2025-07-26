using MVP_Core.Data.Models;
using MVP_Core.Services.System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Services.Admin;
using Services.Diagnostics;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace MVP_Core.Pages.Admin
{
    public class SystemDiagnosticsModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly SystemDiagnosticsService _diagnosticsService;
        private readonly AutoRepairEngine _autoRepairEngine;
        private readonly RootCauseCorrelationEngine _rootCauseCorrelationEngine;
        private readonly SmartAdminAlertsService _smartAdminAlertsService;

        public SystemDiagnosticsModel(ApplicationDbContext db, SystemDiagnosticsService diagnosticsService, AutoRepairEngine autoRepairEngine, RootCauseCorrelationEngine rootCauseCorrelationEngine, SmartAdminAlertsService smartAdminAlertsService)
        {
            _db = db;
            _diagnosticsService = diagnosticsService;
            _autoRepairEngine = autoRepairEngine;
            _rootCauseCorrelationEngine = rootCauseCorrelationEngine;
            _smartAdminAlertsService = smartAdminAlertsService;
        }

        public List<SystemDiagnosticLog> LatestLogs { get; set; } = new();
        public string HealthStatus { get; set; } = "Unknown";
        public string RootCauseSummary { get; set; } = "No summary";
        public List<string> Alerts { get; set; } = new();
        public List<AdminAlertLog> ActiveAlerts { get; set; } = new();
        public string AdminUserId => User?.Identity?.Name ?? "admin";

        public async Task OnGetAsync()
        {
            LatestLogs = await _db.SystemDiagnosticLogs.OrderByDescending(l => l.Timestamp).Take(20).ToListAsync();
            HealthStatus = (await _autoRepairEngine.DetectCorruptionAsync()) ? "Healthy" : "Corruption Detected";
            RootCauseSummary = await _rootCauseCorrelationEngine.CorrelateRootCausesAsync() ?? "No summary";
            Alerts = await _smartAdminAlertsService.TriggerAlertsAsync() ?? new List<string>();
            ActiveAlerts = await _smartAdminAlertsService.GetActiveAlertsAsync(AdminUserId);
        }

        public async Task<IActionResult> OnPostRunDiagnosticsAsync()
        {
            var report = await _diagnosticsService.RunDiagnosticsAsync();
            var log = new SystemDiagnosticLog
            {
                Timestamp = DateTime.UtcNow,
                ModuleName = "SystemDiagnosticsService",
                StatusLevel = report.Status == "Error" ? DiagnosticStatusLevel.Error : report.Status == "Warning" ? DiagnosticStatusLevel.Warning : DiagnosticStatusLevel.OK,
                Summary = $"{report.Status} - {report.ErrorCount} errors, {report.Warnings.Count} warnings",
                Details = System.Text.Json.JsonSerializer.Serialize(report)
            };
            _db.SystemDiagnosticLogs.Add(log);
            await _db.SaveChangesAsync();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRunAutoRepairAsync()
        {
            await _autoRepairEngine.RunAutoRepairAsync(User?.Identity?.Name ?? "admin");
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRewindToSnapshotAsync(int SnapshotId)
        {
            await _autoRepairEngine.RewindToSnapshotAsync(SnapshotId, User?.Identity?.Name ?? "admin");
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostAcknowledgeAlertAsync(int alertId, string actionTaken)
        {
            await _smartAdminAlertsService.AcknowledgeAlertAsync(alertId, AdminUserId, actionTaken);
            return RedirectToPage();
        }
    }
}
