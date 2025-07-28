using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Services.Admin;
using System.Threading.Tasks;

namespace MVP_Core.Pages.Admin
{
    [Authorize(Roles = "Admin,Dispatcher")]
    public class ManagerToolsModel : PageModel
    {
        private readonly ManagerInterventionService _managerService;
        public ManagerToolsModel(ManagerInterventionService managerService)
        {
            _managerService = managerService;
        }

        public void OnGet()
        {
            // Initialization logic will go here
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
