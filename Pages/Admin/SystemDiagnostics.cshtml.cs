using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MVP_Core.Data.Models;
using Services.Admin;
using Services.Diagnostics;
using MVP_Core.Services.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVP_Core.Pages.Admin
{
    public class SystemDiagnosticsModel : PageModel
    {
        private readonly ApplicationDbContext _db; // Sprint 79.2
        private readonly SystemDiagnosticsService _diagnosticsService; // Sprint 79.2
        private readonly AutoRepairEngine _autoRepairEngine; // Sprint 79.2
        private readonly RootCauseCorrelationEngine _rootCauseCorrelationEngine; // Sprint 79.2
        private readonly SmartAdminAlertsService _smartAdminAlertsService; // Sprint 79.2
        private readonly ReplayEngineService _replayEngineService; // Sprint 79.2
        private readonly RecoveryAILearningService _aiService; // Sprint 79.2

        public SystemDiagnosticsModel(ApplicationDbContext db, SystemDiagnosticsService diagnosticsService, AutoRepairEngine autoRepairEngine, RootCauseCorrelationEngine rootCauseCorrelationEngine, SmartAdminAlertsService smartAdminAlertsService, ReplayEngineService replayEngineService)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db)); // Sprint 79.2
            _diagnosticsService = diagnosticsService ?? throw new ArgumentNullException(nameof(diagnosticsService)); // Sprint 79.2
            _autoRepairEngine = autoRepairEngine ?? throw new ArgumentNullException(nameof(autoRepairEngine)); // Sprint 79.2
            _rootCauseCorrelationEngine = rootCauseCorrelationEngine ?? throw new ArgumentNullException(nameof(rootCauseCorrelationEngine)); // Sprint 79.2
            _smartAdminAlertsService = smartAdminAlertsService ?? throw new ArgumentNullException(nameof(smartAdminAlertsService)); // Sprint 79.2
            _replayEngineService = replayEngineService ?? throw new ArgumentNullException(nameof(replayEngineService)); // Sprint 79.2
            _aiService = new RecoveryAILearningService(_db); // Sprint 79.2
            LatestLogs = new(); // Sprint 79.2
            HealthStatus = "Unknown"; // Sprint 79.2
            RootCauseSummary = "No summary"; // Sprint 79.2
            Alerts = new(); // Sprint 79.2
            ActiveAlerts = new(); // Sprint 79.2
            ScheduledScenarios = new(); // Sprint 79.2
            LearnedPatterns = new(); // Sprint 79.2
            SuggestedTriggers = new(); // Sprint 79.2
            StatusMessage = string.Empty; // Sprint 79.2
            MostCommonOutcome = ""; // Sprint 79.2
        }

        public List<SystemDiagnosticLog> LatestLogs { get; set; } = new(); // Sprint 79.2
        public string HealthStatus { get; set; } = "Unknown"; // Sprint 79.2
        public string RootCauseSummary { get; set; } = "No summary"; // Sprint 79.2
        public List<string> Alerts { get; set; } = new(); // Sprint 79.2
        public List<AdminAlertLog> ActiveAlerts { get; set; } = new(); // Sprint 79.2
        public string AdminUserId => User?.Identity?.Name ?? "admin"; // Sprint 79.2
        public List<RecoveryScenarioLog> ScheduledScenarios { get; set; } = new(); // Sprint 79.2
        public List<RecoveryLearningLog> LearnedPatterns { get; set; } = new(); // Sprint 79.2
        [BindProperty]
        public List<string> SuggestedTriggers { get; set; } = new(); // Sprint 79.2
        public string StatusMessage { get; set; } = string.Empty; // Sprint 79.2
        public int TotalPatternsLearned { get; set; } // Sprint 79.2
        public string MostCommonOutcome { get; set; } = ""; // Sprint 79.2
        public RecoveryLearningLog LatestPattern { get; set; } = new RecoveryLearningLog(); // Sprint 79.2

        public async Task<IActionResult> OnGetAsync(string filterRange)
        {
            LatestLogs = await _db.SystemDiagnosticLogs.OrderByDescending(l => l.Timestamp).Take(20).ToListAsync();
            HealthStatus = (await _autoRepairEngine.DetectCorruptionAsync()) ? "Healthy" : "Corruption Detected";
            RootCauseSummary = await _rootCauseCorrelationEngine.CorrelateRootCausesAsync() ?? "No summary";
            Alerts = await _smartAdminAlertsService.TriggerAlertsAsync() ?? new List<string>();
            ActiveAlerts = await _smartAdminAlertsService.GetActiveAlertsAsync(AdminUserId);
            ScheduledScenarios = await _db.RecoveryScenarioLogs.OrderByDescending(s => s.ScheduledForUtc).ToListAsync();

            var query = _db.RecoveryLearningLogs.AsQueryable();
            if (filterRange == "24h")
                query = query.Where(x => x.RecordedAt >= DateTime.UtcNow.AddHours(-24));
            else if (filterRange == "7d")
                query = query.Where(x => x.RecordedAt >= DateTime.UtcNow.AddDays(-7));
            else if (filterRange == "30d")
                query = query.Where(x => x.RecordedAt >= DateTime.UtcNow.AddDays(-30));
            LearnedPatterns = await query.OrderByDescending(x => x.RecordedAt).ToListAsync();
            TotalPatternsLearned = LearnedPatterns.Count;
            MostCommonOutcome = LearnedPatterns.GroupBy(x => x.Outcome).OrderByDescending(g => g.Count()).FirstOrDefault()?.Key ?? "N/A";
            LatestPattern = LearnedPatterns.OrderByDescending(x => x.RecordedAt).FirstOrDefault();
            return Page();
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
            await OnGetAsync("");
            return Page();
        }

        public async Task<IActionResult> OnPostRunAutoRepairAsync()
        {
            await _autoRepairEngine.RunAutoRepairAsync(User?.Identity?.Name ?? "admin");
            await OnGetAsync("");
            return Page();
        }

        public async Task<IActionResult> OnPostRewindToSnapshotAsync(int SnapshotId)
        {
            await _autoRepairEngine.RewindToSnapshotAsync(SnapshotId, User?.Identity?.Name ?? "admin");
            await OnGetAsync("");
            return Page();
        }

        public async Task<IActionResult> OnPostAcknowledgeAlertAsync(int alertId, string actionTaken)
        {
            await _smartAdminAlertsService.AcknowledgeAlertAsync(alertId, AdminUserId, actionTaken);
            await OnGetAsync("");
            return Page();
        }

        public async Task<IActionResult> OnPostQueueRecoveryAsync(string ScenarioName, DateTime ScheduledForUtc, string SnapshotHash, string Notes)
        {
            await _replayEngineService.QueueRecoveryScenarioAsync(ScenarioName, AdminUserId, ScheduledForUtc, SnapshotHash, Notes);
            await OnGetAsync("");
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
            await OnGetAsync("");
            return Page();
        }

        public async Task<IActionResult> OnPostAnalyzePatternsAsync()
        {
            StatusMessage = "Analyzing patterns...";
            var learned = await _aiService.AnalyzeRecoveryPatternsAsync();
            StatusMessage = "Pattern analysis complete.";
            LearnedPatterns = learned;
            TotalPatternsLearned = LearnedPatterns.Count;
            MostCommonOutcome = LearnedPatterns.GroupBy(x => x.Outcome).OrderByDescending(g => g.Count()).FirstOrDefault()?.Key ?? "N/A";
            LatestPattern = LearnedPatterns.OrderByDescending(x => x.RecordedAt).FirstOrDefault();
            await OnGetAsync("");
            return Page();
        }

        public async Task<IActionResult> OnPostSuggestTriggersAsync()
        {
            StatusMessage = "Suggesting triggers...";
            SuggestedTriggers = await _aiService.SuggestAutoRepairTriggers();
            StatusMessage = "Trigger suggestion complete.";
            await OnGetAsync("");
            return Page();
        }
    }
}
