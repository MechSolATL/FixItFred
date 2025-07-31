using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Data;
using Services.Admin;

namespace Pages.Admin
{
    public class ScheduleConflictModel : PageModel
    {
        private readonly ScheduleConflictDetectionService _conflictService;
        private readonly ApplicationDbContext _db;
        public ScheduleConflictModel(ScheduleConflictDetectionService conflictService, ApplicationDbContext db)
        {
            _conflictService = conflictService;
            _db = db;
            Conflicts = new List<TechnicianScheduleConflictLog>();
        }
        public List<TechnicianScheduleConflictLog> Conflicts { get; set; }
        public async Task OnGetAsync()
        {
            Conflicts = await _conflictService.GetConflictsAsync();
        }
        public async Task<IActionResult> OnPostDetectAsync()
        {
            var techIds = await _db.Technicians.Select(t => t.Id).ToListAsync();
            foreach (var techId in techIds)
            {
                await _conflictService.DetectConflictsAsync(techId);
            }
            Conflicts = await _conflictService.GetConflictsAsync();
            return Page();
        }
        public async Task<IActionResult> OnPostAcknowledgeAsync(int conflictId, string note)
        {
            await _conflictService.AcknowledgeConflictAsync(conflictId, note);
            Conflicts = await _conflictService.GetConflictsAsync();
            return Page();
        }
        public async Task<IActionResult> OnPostResolveAsync(int conflictId, string suggestion)
        {
            await _conflictService.ResolveConflictAsync(conflictId, suggestion);
            Conflicts = await _conflictService.GetConflictsAsync();
            return Page();
        }
    }
}
