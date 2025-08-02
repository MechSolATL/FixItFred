using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;
// FixItFred: Remove all ambiguous using for TechnicianAuditService, use fully qualified name
using DomainTechnicianBehaviorLog = Models.TechnicianBehaviorLog;
using Data.Models;
using AdminTechnicianAuditService = Services.Admin.TechnicianAuditService;
using AdminPermissionService = Services.Admin.PermissionService;

namespace Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class FlagReviewQueueModel : PageModel
    {
        private readonly AdminTechnicianAuditService _auditService;
        // [Sprint123_FixItFred] Using aliased namespace references to resolve compiler ambiguity
        public AdminPermissionService PermissionService { get; }
        public AdminUser AdminUser { get; }
        public List<DomainTechnicianBehaviorLog> FlaggedLogs { get; set; } = new();
        public FlagReviewQueueModel(AdminTechnicianAuditService auditService, AdminPermissionService permissionService)
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
