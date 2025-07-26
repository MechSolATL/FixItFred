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
        public SystemValidationModel(ApplicationDbContext db, ValidationSimulatorService simService)
        {
            _db = db;
            _simService = simService;
        }

        public string LastOutcome { get; set; } = "";
        public string LastOutcomeTimestamp { get; set; } = "";
        public List<SystemSnapshotLog> LastSnapshots { get; set; } = new();
        public List<AdminAlertLog> LastAlerts { get; set; } = new();

        public async Task OnGetAsync()
        {
            LastSnapshots = _db.SystemSnapshotLogs.OrderByDescending(s => s.Timestamp).Take(5).ToList();
            LastAlerts = _db.AdminAlertLogs.OrderByDescending(a => a.Timestamp).Take(10).ToList();
            // Only valid properties are used in Razor/UI
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
    }
}
