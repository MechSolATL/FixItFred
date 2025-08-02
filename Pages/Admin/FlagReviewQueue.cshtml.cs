using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;
// FixItFred: Remove all ambiguous using for TechnicianAuditService, use fully qualified name
using DomainTechnicianBehaviorLog = Models.TechnicianBehaviorLog;
using Data.Models;
using Services.Admin;

namespace Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class FlagReviewQueueModel : PageModel
    {
        private readonly Services.Admin.TechnicianAuditService _auditService;
        // [Sprint1002_FixItFred] Fully qualified to resolve namespace reference
        public Services.Admin.PermissionService PermissionService { get; }
        public AdminUser AdminUser { get; }
        public List<DomainTechnicianBehaviorLog> FlaggedLogs { get; set; } = new();
        public FlagReviewQueueModel(Services.Admin.TechnicianAuditService auditService, Services.Admin.PermissionService permissionService)
        {
            _auditService = auditService;
            PermissionService = permissionService;
            AdminUser = HttpContext?.Items["AdminUser"] as AdminUser ?? new AdminUser { EnabledModules = new List<string>() };
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
