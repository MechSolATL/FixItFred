using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Data;
using MVP_Core.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace MVP_Core.Pages.Admin
{
    public class BreakComplianceModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public BreakComplianceModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public List<TechnicianBreakStatus> PendingBreaks { get; set; } = new();
        public List<BreakComplianceOverrideLog> OverrideLogs { get; set; } = new();
        public List<TechnicianBreakStatus> CountdownTechs { get; set; } = new();
        public List<AuditPanelEntry> AuditPanel { get; set; } = new();
        public string HRAlert { get; set; } = string.Empty;

        public async Task OnGetAsync()
        {
            var now = DateTime.UtcNow;
            // Get all active technicians
            var techs = await _db.Technicians.Where(t => t.IsActive).ToListAsync();
            // Get last session telemetry for each tech
            var sessions = await _db.TechnicianSessionTelemetries
                .GroupBy(s => s.TechnicianId)
                .Select(g => g.OrderByDescending(s => s.EndTime ?? s.StartTime).FirstOrDefault())
                .ToListAsync();
            // Get override logs for audit
            OverrideLogs = await _db.BreakComplianceOverrideLogs.ToListAsync();
            // Build break status for each tech
            foreach (var tech in techs)
            {
                var session = sessions.FirstOrDefault(s => s.TechnicianId == tech.Id);
                var lastBreak = session?.EndTime ?? session?.StartTime ?? DateTime.MinValue;
                var hoursSinceBreak = (now - lastBreak).TotalHours;
                var status = new TechnicianBreakStatus
                {
                    TechnicianId = tech.Id,
                    TechnicianName = tech.FullName,
                    HoursSinceBreak = hoursSinceBreak,
                    LastBreak = lastBreak,
                    IsCountdown = hoursSinceBreak >= 4.5 && hoursSinceBreak < 5,
                    IsLockout = hoursSinceBreak >= 5
                };
                if (status.IsCountdown)
                    CountdownTechs.Add(status);
                if (status.IsLockout)
                    PendingBreaks.Add(status);
            }
            // Audit panel: show override history, filter, heatmap
            AuditPanel = OverrideLogs
                .GroupBy(l => l.TechnicianId)
                .Select(g => new AuditPanelEntry
                {
                    TechnicianId = g.Key,
                    TechnicianName = techs.FirstOrDefault(t => t.Id == g.Key)?.FullName ?? "Unknown",
                    OverrideCount = g.Count(),
                    EmergencyCount = g.Count(l => l.WasEmergency),
                    AutoFlaggedCount = g.Count(l => l.AutoFlagged),
                    LastOverride = g.Max(l => l.Timestamp)
                }).ToList();
            // Auto-flag detection: >2 overrides in 7 days
            var weekAgo = now.AddDays(-7);
            var flaggedTechs = OverrideLogs
                .Where(l => l.Timestamp > weekAgo)
                .GroupBy(l => l.TechnicianId)
                .Where(g => g.Count() > 2)
                .Select(g => g.Key)
                .ToList();
            foreach (var techId in flaggedTechs)
            {
                foreach (var log in OverrideLogs.Where(l => l.TechnicianId == techId && l.Timestamp > weekAgo))
                {
                    log.AutoFlagged = true;
                }
                HRAlert = "Compliance Override Threshold Breached — Review Suggested";
            }
            // Save auto-flag updates
            await _db.SaveChangesAsync();
        }

        public async Task<IActionResult> OnPostOverrideAsync(int technicianId, string justification, bool wasEmergency)
        {
            // Only HR/Admin/Manager can override
            var user = await _db.AdminUsers.FirstOrDefaultAsync(u => u.Username == User.Identity.Name);
            if (user == null || !(user.Role == "HR" || user.Role == "Admin" || user.Role == "Manager"))
                return Forbid();
            // Log override
            var log = new BreakComplianceOverrideLog
            {
                TechnicianId = technicianId,
                ApprovedByUserId = user.Id,
                RoleOfApprover = user.Role ?? "Unknown",
                Justification = justification,
                Timestamp = DateTime.UtcNow,
                DurationExtended = 60, // Extend by 1 hour
                WasEmergency = wasEmergency,
                AutoFlagged = false
            };
            _db.BreakComplianceOverrideLogs.Add(log);
            await _db.SaveChangesAsync();
            return RedirectToPage();
        }

        public class TechnicianBreakStatus
        {
            public int TechnicianId { get; set; }
            public string TechnicianName { get; set; } = string.Empty;
            public double HoursSinceBreak { get; set; }
            public DateTime LastBreak { get; set; }
            public bool IsCountdown { get; set; }
            public bool IsLockout { get; set; }
        }
        public class AuditPanelEntry
        {
            public int TechnicianId { get; set; }
            public string TechnicianName { get; set; } = string.Empty;
            public int OverrideCount { get; set; }
            public int EmergencyCount { get; set; }
            public int AutoFlaggedCount { get; set; }
            public DateTime LastOverride { get; set; }
        }
    }
}
