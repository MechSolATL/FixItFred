using MVP_Core.Data.Models;
using MVP_Core.Services.System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MVP_Core.Pages.Admin
{
    public class SystemDiagnosticsModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly SystemDiagnosticsService _diagnosticsService;

        public SystemDiagnosticsModel(ApplicationDbContext db, SystemDiagnosticsService diagnosticsService)
        {
            _db = db;
            _diagnosticsService = diagnosticsService;
        }

        public List<SystemDiagnosticLog> LatestLogs { get; set; } = new();

        public async Task OnGetAsync()
        {
            LatestLogs = await _db.SystemDiagnosticLogs.OrderByDescending(l => l.Timestamp).Take(20).ToListAsync();
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
    }
}
