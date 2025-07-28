using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;
// FixItFred: Remove all ambiguous using for TechnicianAuditService, use fully qualified name
using DomainTechnicianBehaviorLog = MVP_Core.Models.TechnicianBehaviorLog;

namespace MVP_Core.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class FlagReviewQueueModel : PageModel
    {
        private readonly MVP_Core.Services.Admin.TechnicianAuditService _auditService;
        public List<DomainTechnicianBehaviorLog> FlaggedLogs { get; set; } = new();
        public FlagReviewQueueModel(MVP_Core.Services.Admin.TechnicianAuditService auditService)
        {
            _auditService = auditService;
        }
        public async Task OnGetAsync()
        {
            FlaggedLogs = await _auditService.GetFlaggedLogsAsync();
        }
        public async Task<IActionResult> OnPostClearAsync(int id)
        {
            await _auditService.UpdateLogStatusAsync(id, "Cleared");
            return RedirectToPage();
        }
        public async Task<IActionResult> OnPostEscalateAsync(int id)
        {
            await _auditService.UpdateLogStatusAsync(id, "Escalated");
            return RedirectToPage();
        }
        public async Task<IActionResult> OnPostAddNoteAsync(int id, string note)
        {
            await _auditService.UpdateLogStatusAsync(id, "Flagged", note);
            return RedirectToPage();
        }
    }
}
