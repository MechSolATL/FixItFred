// Sprint 85.4 — Coaching UI Enhancements + Export
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Models;
using MVP_Core.Services.Admin;
using MVP_Core.Data.Models;
using System.Collections.Generic;
using System.Linq;
using System;

namespace MVP_Core.Pages.Admin
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
        public List<CoachingLogEntry> Entries { get; set; } = new();
        public List<Technician> Technicians { get; set; } = new();
        public List<string> Tiers { get; set; } = new();
        public List<string> Supervisors { get; set; } = new();
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
            // Sprint 85.4 — Coaching UI Enhancements + Export
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
        }
        public IActionResult OnGetExport(string startDate, string endDate, string tier, string supervisor)
        {
            // Sprint 85.4 — Coaching UI Enhancements + Export
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
            var csv = Services.Admin.CsvExportHelper.ExportCoachingLogs(filtered);
            return File(System.Text.Encoding.UTF8.GetBytes(csv), "text/csv", $"CoachingLogs_{DateTime.UtcNow:yyyyMMdd}.csv");
        }
        public IActionResult OnPost()
        {
            // Sprint 85.4 — Coaching UI Enhancements + Export
            var techId = int.Parse(Request.Form["TechnicianId"]);
            var supervisor = Request.Form["SupervisorName"];
            var category = Request.Form["Category"];
            var note = Request.Form["CoachingNote"];
            _logService.AddEntry(new CoachingLogEntry
            {
                TechnicianId = techId,
                SupervisorName = supervisor,
                Category = category,
                CoachingNote = note
            });
            return RedirectToPage();
        }
    }
}
