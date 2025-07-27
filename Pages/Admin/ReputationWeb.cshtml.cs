using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Services.Admin;
using MVP_Core.Data;
using MVP_Core.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MVP_Core.Pages.Admin
{
    public class ReputationWebModel : PageModel
    {
        private readonly ReputationGraphBuilderService _graphService;
        public List<TechnicianReputationEdge> ReputationEdges { get; set; } = new();
        public ReputationWebModel(ReputationGraphBuilderService graphService)
        {
            _graphService = graphService;
        }
        public async Task OnGetAsync()
        {
            ReputationEdges = await _graphService.BuildGraphAsync();
        }
    }
}
