using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Data.Models;
using Services.Admin;

namespace Pages.Admin
{
    public class ConflictRadarModel : PageModel
    {
        private readonly TechnicianConflictRadarService _conflictService;
        public ConflictRadarModel(TechnicianConflictRadarService conflictService)
        {
            _conflictService = conflictService;
        }

        [BindProperty(SupportsGet = true)]
        public int SeverityFilter { get; set; }
        public List<TechnicianConflictLog> ConflictLogs { get; set; } = new();

        public async Task OnGetAsync()
        {
            var logs = await _conflictService.GetActiveConflictsAsync();
            if (SeverityFilter > 0)
                logs = logs.Where(x => x.SeverityLevel == SeverityFilter).ToList();
            ConflictLogs = logs;
        }

        public async Task<IActionResult> OnPostAsync(int ConflictId, string ResolutionNotes)
        {
            await _conflictService.ResolveConflictAsync(ConflictId, ResolutionNotes);
            return RedirectToPage();
        }
    }
}
