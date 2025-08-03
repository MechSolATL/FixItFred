// Sprint 85.6 � Coaching Impact Insights Phase 2
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using Helpers;
using Models;
using Data;
using Services.Admin;

namespace Pages.Admin
{
    public class CoachingLogModel : PageModel
    {
        private readonly CoachingLogService _logService;
        private readonly ApplicationDbContext _db;
        public CoachingLogModel(CoachingLogService logService, ApplicationDbContext db)
        {
            _logService = logService;
            _db = db;
        }
        public List<Data.Models.Technician> Technicians { get; set; } = new();
        public List<string> Tiers { get; set; } = new();
        public List<string> Supervisors { get; set; } = new();
        public List<CoachingLogEntry> Entries { get; set; } = new();
        public double AvgTrustImprovement { get; set; }
        public Dictionary<int, int> TechnicianTrustImprovements { get; set; } = new();
        [BindProperty(SupportsGet = true)]
        public string StartDateString { get; set; } = string.Empty;
        [BindProperty(SupportsGet = true)]
        public string EndDateString { get; set; } = string.Empty;
        [BindProperty(SupportsGet = true)]
        public string SelectedTier { get; set; } = string.Empty;
        [BindProperty(SupportsGet = true)]
        public string SelectedSupervisor { get; set; } = string.Empty;
        public void OnGet()
        {
            // Sprint 85.6 � Coaching Impact Insights Phase 2
            Technicians = _db.Technicians.ToList();
            Tiers = Technicians.Select(t => t.TierLevel.ToString()).Distinct().OrderBy(x => x).ToList();
            Supervisors = _logService.FilterByDate(DateTime.MinValue, DateTime.MaxValue).Select(e => e.SupervisorName).Distinct().OrderBy(x => x).ToList();
            DateTime start = DateTime.TryParse(StartDateString, out var s) ? s : DateTime.MinValue;
            DateTime end = DateTime.TryParse(EndDateString, out var e) ? e : DateTime.MaxValue;
            var filtered = _logService.FilterByDate(start, end);
            if (!string.IsNullOrEmpty(SelectedTier))
            {
                var techIds = Technicians.Where(t => t.TierLevel.ToString() == SelectedTier).Select(t => t.Id).ToList();
                filtered = filtered.Where(e => techIds.Contains(e.TechnicianId)).ToList();
            }
            if (!string.IsNullOrEmpty(SelectedSupervisor))
            {
                filtered = filtered.Where(e => e.SupervisorName == SelectedSupervisor).ToList();
            }
            Entries = filtered;
            // Calculate average trust improvement
            AvgTrustImprovement = _logService.GetAverageTrustImprovementPerTechnician();
            // Calculate per-technician trust improvement
            TechnicianTrustImprovements = Technicians.ToDictionary(
                t => t.Id,
                t => _logService.GetTotalTrustImprovementForTechnician(t.Id)
            );
        }
        public IActionResult OnGetExport(string startDate, string endDate, string tier, string supervisor)
        {
            // Sprint 85.4 � Coaching UI Enhancements + Export
            DateTime start = DateTime.TryParse(startDate, out var s) ? s : DateTime.MinValue;
            DateTime end = DateTime.TryParse(endDate, out var e) ? e : DateTime.MaxValue;
            var filtered = _logService.FilterByDate(start, end);
            if (!string.IsNullOrEmpty(tier))
            {
                var techIds = _db.Technicians.Where(t => t.TierLevel.ToString() == tier).Select(t => t.Id).ToList();
                filtered = filtered.Where(e => techIds.Contains(e.TechnicianId)).ToList();
            }
            if (!string.IsNullOrEmpty(supervisor))
            {
                filtered = filtered.Where(e => e.SupervisorName == supervisor).ToList();
            }
            var csv = global::Services.Admin.CsvExportHelper.ExportCoachingLogs(filtered); // FixItFred: Use global:: to avoid namespace conflicts
            return File(global::System.Text.Encoding.UTF8.GetBytes(csv), "text/csv", $"CoachingLogs_{DateTime.UtcNow:yyyyMMdd}.csv"); // FixItFred: Use global:: for System.Text
        }
        public IActionResult OnPost()
        {
            // Sprint 85.7 � Admin Log Hardening & Encryption
            var techId = int.Parse(Request.Form["TechnicianId"]);
            var supervisor = Request.Form["SupervisorName"];
            var category = Request.Form["Category"];
            var note = Request.Form["CoachingNote"];
            var isSensitive = !string.IsNullOrEmpty(Request.Form["IsSensitive"]) && Request.Form["IsSensitive"] == "on";
            _logService.AddEntry(new CoachingLogEntry
            {
                TechnicianId = techId,
                SupervisorName = supervisor,
                Category = category,
                CoachingNote = note,
                IsSensitive = isSensitive
            });
            return RedirectToPage();
        }
        // Helper to decrypt note for admin
        public string GetDecryptedNote(CoachingLogEntry entry)
        {
            if (entry.IsSensitive && User.IsInRole("Admin"))
            {
                try { return SensitiveNoteEncryptor.Decrypt(entry.CoachingNote); } catch { return "[Decryption Error]"; }
            }
            return entry.CoachingNote;
        }
    }
}
