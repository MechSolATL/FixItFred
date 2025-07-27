using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Services.Admin;
using MVP_Core.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MVP_Core.Pages.Admin
{
    public class IncidentClustersModel : PageModel
    {
        private readonly IncidentCompressionService _incidentCompressionService;
        public IncidentClustersModel(IncidentCompressionService incidentCompressionService)
        {
            _incidentCompressionService = incidentCompressionService;
        }
        [BindProperty]
        public string ClusterKey { get; set; } = string.Empty;
        [BindProperty]
        public int OccurrenceCount { get; set; }
        [BindProperty]
        public string? EquipmentFaults { get; set; }
        [BindProperty]
        public string? DispatchIssues { get; set; }
        [BindProperty]
        public bool TechBurnoutSuspected { get; set; }
        [BindProperty]
        public bool ClientAbuseSuspected { get; set; }
        [BindProperty]
        public string? Notes { get; set; }
        public List<IncidentCompressionLog> IncidentClusters { get; set; } = new();
        public async Task OnGetAsync()
        {
            IncidentClusters = await _incidentCompressionService.GetIncidentClustersAsync();
        }
        public async Task<IActionResult> OnPostAddClusterAsync()
        {
            await _incidentCompressionService.AddIncidentClusterAsync(ClusterKey, OccurrenceCount, EquipmentFaults, DispatchIssues, TechBurnoutSuspected, ClientAbuseSuspected, Notes);
            IncidentClusters = await _incidentCompressionService.GetIncidentClustersAsync();
            return Page();
        }
    }
}
