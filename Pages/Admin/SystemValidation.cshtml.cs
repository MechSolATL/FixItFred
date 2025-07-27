using MVP_Core.Data;
using MVP_Core.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVP_Core.Pages.Admin
{
    public class SystemValidationModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly ValidationSimulatorService _simService;
        private readonly ReplayEngineService _replayService;
        public SystemValidationModel(ApplicationDbContext db, ValidationSimulatorService simService, ReplayEngineService replayService)
        {
            _db = db;
            _simService = simService;
            _replayService = replayService;
        }

        public string LastOutcome { get; set; } = "";
        public string LastOutcomeTimestamp { get; set; } = "";
        public List<SystemSnapshotLog> LastSnapshots { get; set; } = new();
        public List<AdminAlertLog> LastAlerts { get; set; } = new();
        public List<ReplayAuditLog> ReplayAuditLogs { get; set; } = new();

        public async Task OnGetAsync()
        {
            LastSnapshots = _db.SystemSnapshotLogs.OrderByDescending(s => s.Timestamp).Take(5).ToList();
            LastAlerts = _db.AdminAlertLogs.OrderByDescending(a => a.Timestamp).Take(10).ToList();
            ReplayAuditLogs = _db.ReplayAuditLogs.OrderByDescending(r => r.Timestamp).Take(10).ToList();
        }

        public async Task<IActionResult> OnPostRunDiagnosticsAsync()
        {
            LastOutcome = await _simService.RunSimulatedDiagnosticsAsync();
            LastOutcomeTimestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
            await OnGetAsync();
            return Page();
        }
        public async Task<IActionResult> OnPostTriggerAlertAsync()
        {
            LastOutcome = await _simService.TriggerSyntheticAlertAsync();
            LastOutcomeTimestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
            await OnGetAsync();
            return Page();
        }
        public async Task<IActionResult> OnPostSimulateStorageSpikeAsync()
        {
            LastOutcome = await _simService.SimulateStorageSpikeAsync();
            LastOutcomeTimestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
            await OnGetAsync();
            return Page();
        }
        public async Task<IActionResult> OnPostSimulateSLADriftAsync()
        {
            LastOutcome = await _simService.SimulateSLADriftAsync();
            LastOutcomeTimestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
            await OnGetAsync();
            return Page();
        }
        public async Task<IActionResult> OnPostRestoreSnapshotAsync()
        {
            LastOutcome = await _simService.SimulateSnapshotRestoreAsync();
            LastOutcomeTimestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
            await OnGetAsync();
            return Page();
        }
        public async Task<IActionResult> OnPostRunReplayAsync(string snapshotHash, DateTime? overrideTimestamp, bool autoHealSim)
        {
            var notes = autoHealSim ? "Auto-Heal simulation enabled" : null;
            var success = await _replayService.ReplaySnapshotAsync(snapshotHash, User.Identity?.Name ?? "admin", overrideTimestamp, notes);
            LastOutcome = success ? "Replay successful." : "Replay failed.";
            LastOutcomeTimestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
            await OnGetAsync();
            return Page();
        }
        public async Task<IActionResult> OnPostValidateAsync()
        {
            // Add 'await' to make this method truly async
            await Task.CompletedTask;
            return Page();
        }
    }
}
