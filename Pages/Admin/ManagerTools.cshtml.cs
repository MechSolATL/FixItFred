using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Services.Admin;
using MVP_Core.Services; // For INotificationService
using MVP_Core.Services; // For AuditLogger
using System.Threading.Tasks;

namespace MVP_Core.Pages.Admin
{
    [Authorize(Roles = "Admin,Dispatcher")]
    public class ManagerToolsModel : PageModel
    {
        private readonly ManagerInterventionService _managerService;
        private readonly INotificationService _notificationService;
        private readonly AuditLogger _auditLogger;
        public ManagerToolsModel(ManagerInterventionService managerService, INotificationService notificationService, AuditLogger auditLogger)
        {
            _managerService = managerService;
            _notificationService = notificationService;
            _auditLogger = auditLogger;
        }

        public void OnGet()
        {
            // Initialization logic will go here
        }

        // Sprint 91.7.8.2: POST handlers for manager actions with audit/notification
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostCancelRequestAsync(int requestId, string reason)
        {
            var result = await _managerService.CancelRequestAsync(requestId, reason, User.Identity?.Name ?? "system", _notificationService, _auditLogger, HttpContext.Connection.RemoteIpAddress?.ToString() ?? "");
            TempData["ManagerActionResult"] = result ? "Request cancelled successfully." : "Failed to cancel request.";
            return RedirectToPage();
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostReassignTechAsync(int requestId, int newTechId)
        {
            var result = await _managerService.ReassignTechnicianAsync(requestId, newTechId, User.Identity?.Name ?? "system", _notificationService, _auditLogger, HttpContext.Connection.RemoteIpAddress?.ToString() ?? "");
            TempData["ManagerActionResult"] = result ? "Technician reassigned successfully." : "Failed to reassign technician.";
            return RedirectToPage();
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostReopenTicketAsync(int requestId, string managerNotes)
        {
            var result = await _managerService.ReopenTicketAsync(requestId, managerNotes, User.Identity?.Name ?? "system", _notificationService, _auditLogger, HttpContext.Connection.RemoteIpAddress?.ToString() ?? "");
            TempData["ManagerActionResult"] = result ? "Ticket reopened successfully." : "Failed to reopen ticket.";
            return RedirectToPage();
        }

        // Sprint 91.7.8.1: Async handlers for manager interventions
        public async Task<IActionResult> OnPostCancelAssignmentAsync(int requestId)
        {
            await _managerService.CancelTechnicianAssignment(requestId);
            return RedirectToPage();
        }
        public async Task<IActionResult> OnPostForceAssignAsync(int requestId, int technicianId)
        {
            await _managerService.ForceAssignTechnician(requestId, technicianId);
            return RedirectToPage();
        }
        public async Task<IActionResult> OnPostResetRouteAsync(int technicianId)
        {
            await _managerService.ResetRoute(technicianId);
            return RedirectToPage();
        }
        public async Task<IActionResult> OnPostReopenRequestAsync(int requestId)
        {
            await _managerService.ReopenRequest(requestId);
            return RedirectToPage();
        }
        public async Task<IActionResult> OnPostFlagTechnicianAsync(int technicianId, string reason)
        {
            await _managerService.FlagTechnician(technicianId, reason);
            return RedirectToPage();
        }
    }
}
