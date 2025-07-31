using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Services.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MVP_Core.Services.Admin;
using Interfaces;
using Services.System;
using Data;
using Data.Models;

namespace Pages.Admin
{
    public class SystemDiagnosticsModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly SystemDiagnosticsService _diagnosticsService;
        private readonly AutoRepairEngine _autoRepairEngine;
        private readonly IRootCauseCorrelationEngine _rootCauseCorrelationEngine;
        private readonly ISmartAdminAlertsService _smartAdminAlertsService;
        private readonly IReplayEngineService _replayEngineService;
        private readonly RecoveryAILearningService _aiService;

        public SystemDiagnosticsModel(
            ApplicationDbContext db,
            SystemDiagnosticsService diagnosticsService,
            AutoRepairEngine autoRepairEngine,
            IRootCauseCorrelationEngine rootCauseCorrelationEngine,
            ISmartAdminAlertsService smartAdminAlertsService,
            IReplayEngineService replayEngineService)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _diagnosticsService = diagnosticsService ?? throw new ArgumentNullException(nameof(diagnosticsService));
            _autoRepairEngine = autoRepairEngine ?? throw new ArgumentNullException(nameof(autoRepairEngine));
            _rootCauseCorrelationEngine = rootCauseCorrelationEngine ?? throw new ArgumentNullException(nameof(rootCauseCorrelationEngine));
            _smartAdminAlertsService = smartAdminAlertsService ?? throw new ArgumentNullException(nameof(smartAdminAlertsService));
            _replayEngineService = replayEngineService ?? throw new ArgumentNullException(nameof(replayEngineService));
            _aiService = new RecoveryAILearningService(_db);
        }

        public List<SystemDiagnosticLog> LatestLogs { get; set; } = new();
        public string HealthStatus { get; set; } = "Unknown";
        public string RootCauseSummary { get; set; } = "No summary";
        public List<string> Alerts { get; set; } = new();
        public List<AdminAlertLog> ActiveAlerts { get; set; } = new();
        public string AdminUserId => User?.Identity?.Name ?? "admin";
        public List<RecoveryScenarioLog> ScheduledScenarios { get; set; } = new();
        public List<RecoveryLearningLog> LearnedPatterns { get; set; } = new();
        [BindProperty]
        public List<string> SuggestedTriggers { get; set; } = new();
        public string StatusMessage { get; set; } = string.Empty;
        public int TotalPatternsLearned { get; set; }
        public string MostCommonOutcome { get; set; } = "";
        public RecoveryLearningLog LatestPattern { get; set; } = new RecoveryLearningLog();
        public string ViewTitle { get; set; } = "Untitled";

        // [Sprint91_26] SEO Razor Binding Fix
        public SeoMetadata Seo { get; set; } = new SeoMetadata();
        public string Title => Seo.Title;
        public string MetaDescription => Seo.MetaDescription;
        public string Keywords => Seo.Keywords;
        public string Robots => Seo.Robots;

        public async Task<IActionResult> OnGetAsync(string filterRange)
        {
            LatestLogs = await _db.SystemDiagnosticLogs.OrderByDescending(l => l.Timestamp).Take(20).ToListAsync();
            HealthStatus = await _autoRepairEngine.DetectCorruptionAsync() ? "Healthy" : "Corruption Detected";
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
            await _autoRepairEngine.RunAutoRepairAsync(AdminUserId);
            await OnGetAsync("");
            return Page();
        }

        public async Task<IActionResult> OnPostRewindToSnapshotAsync(int SnapshotId)
        {
            await _autoRepairEngine.RewindToSnapshotAsync(SnapshotId, AdminUserId);
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
                var success = await _replayEngineService.ReplaySnapshotAsync(
                    scenario.SnapshotHash,
                    scenario.TriggeredBy,
                    DateTime.UtcNow,
                    scenario.Notes
                );
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
