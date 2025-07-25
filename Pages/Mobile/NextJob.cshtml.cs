// [FixItFred] Stabilization Patch: Purpose, service/model usage, and fix summary
// Purpose: Explicitly reference MVP_Core.Models.Mobile.NextJobDto to resolve ambiguity.
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Services.Admin;

namespace MVP_Core.Pages.Mobile
{
    public class NextJobModel : PageModel
    {
        private readonly DispatcherService _dispatcherService;
        public NextJobModel(DispatcherService dispatcherService)
        {
            _dispatcherService = dispatcherService;
        }
        public MVP_Core.Models.Mobile.NextJobDto? Job { get; private set; }
        public int TechId { get; private set; }
        public void OnGet()
        {
            TechId = int.TryParse(Request.Query["techId"], out var tId) ? tId : 0;
            Job = _dispatcherService.GetNextJobForTechnician(TechId);
        }
        public void OnPostPing()
        {
            _dispatcherService.UpdateTechnicianPing(TechId);
            Job = _dispatcherService.GetNextJobForTechnician(TechId);
        }
    }
}
