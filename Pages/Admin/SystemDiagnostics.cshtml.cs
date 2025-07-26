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
using System.Linq;

namespace MVP_Core.Pages.Admin
{
    public class SystemDiagnosticsModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly SystemDiagnosticsService _diagnosticsService;
        private readonly AutoRepairEngine _autoRepairEngine;
        private readonly RootCauseCorrelationEngine _rootCauseCorrelationEngine;
        private readonly SmartAdminAlertsService _smartAdminAlertsService;
        private readonly ReplayEngineService _replayEngineService;

        public SystemDiagnosticsModel(ApplicationDbContext db, SystemDiagnosticsService diagnosticsService, AutoRepairEngine autoRepairEngine, RootCauseCorrelationEngine rootCauseCorrelationEngine, SmartAdminAlertsService smartAdminAlertsService, ReplayEngineService replayEngineService)
        {
            _db = db;
            _diagnosticsService = diagnosticsService;
            _autoRepairEngine = autoRepairEngine;
            _rootCauseCorrelationEngine = rootCauseCorrelationEngine;
            _smartAdminAlertsService = smartAdminAlertsService;
            _replayEngineService = replayEngineService;
        }

        public List<SystemDiagnosticLog> LatestLogs { get; set; } = new();
        public string HealthStatus { get; set; } = "Unknown";
        public string RootCauseSummary { get; set; } = "No summary";
        public List<string> Alerts { get; set; } = new();
        public List<AdminAlertLog> ActiveAlerts { get; set; } = new();
        public string AdminUserId => User?.Identity?.Name ?? "admin";
        public List<RecoveryScenarioLog> ScheduledScenarios { get; set; } = new();

        public async Task OnGetAsync()
        {
            LatestLogs = await _db.SystemDiagnosticLogs.OrderByDescending(l => l.Timestamp).Take(20).ToListAsync();
            HealthStatus = (await _autoRepairEngine.DetectCorruptionAsync()) ? "Healthy" : "Corruption Detected";
            RootCauseSummary = await _rootCauseCorrelationEngine.CorrelateRootCausesAsync() ?? "No summary";
            Alerts = await _smartAdminAlertsService.TriggerAlertsAsync() ?? new List<string>();
            ActiveAlerts = await _smartAdminAlertsService.GetActiveAlertsAsync(AdminUserId);
            ScheduledScenarios = await _db.RecoveryScenarioLogs.OrderByDescending(s => s.ScheduledForUtc).ToListAsync();
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

        public async Task<IActionResult> OnPostQueueRecoveryAsync(string ScenarioName, DateTime ScheduledForUtc, string SnapshotHash, string Notes)
        {
            await _replayEngineService.QueueRecoveryScenarioAsync(ScenarioName, AdminUserId, ScheduledForUtc, SnapshotHash, Notes);
            await OnGetAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostExecuteNowAsync(int id)
        {
            var scenario = await _db.RecoveryScenarioLogs.FindAsync(id);
            if (scenario != null && !scenario.Executed)
            {
                var success = await _replayEngineService.ReplaySnapshotAsync(scenario.SnapshotHash, scenario.TriggeredBy, DateTime.UtcNow, scenario.Notes);
                scenario.Executed = true;
                scenario.ExecutedAtUtc = DateTime.UtcNow;
                scenario.OutcomeSummary = success ? "Replay succeeded" : "Replay failed";
                _db.RecoveryScenarioLogs.Update(scenario);
                await _db.SaveChangesAsync();
            }
            await OnGetAsync();
            return Page();
        }
    }
}
