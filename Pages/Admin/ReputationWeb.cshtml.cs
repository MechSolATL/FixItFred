using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Models;
using Services.Admin;

namespace Pages.Admin
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
