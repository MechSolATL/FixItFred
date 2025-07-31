using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;
using Services.Admin;

namespace Pages.Admin
{
    /// <summary>
    /// PageModel for Technician Load Heatmap view. Pulls technician load summary and supports live updates.
    /// </summary>
    public class TechnicianLoadHeatmapModel : PageModel
    {
        private readonly DispatcherService _dispatcherService;
        public List<DispatcherService.TechnicianLoadSummaryDto> TechnicianLoads { get; set; } = new();

        public TechnicianLoadHeatmapModel(DispatcherService dispatcherService)
        {
            _dispatcherService = dispatcherService;
        }

        public async Task OnGetAsync()
        {
            TechnicianLoads = await _dispatcherService.GetTechnicianLoadSummaryAsync();
        }
    }
}
