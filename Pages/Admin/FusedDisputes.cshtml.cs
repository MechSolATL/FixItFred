using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Models;
using Services.Admin;

namespace Pages.Admin
{
    public class FusedDisputesModel : PageModel
    {
        private readonly DisputeFusionAnalyzerService _fusionService;
        public List<DisputeFusionLog> FusedDisputes { get; set; } = new();
        public FusedDisputesModel(DisputeFusionAnalyzerService fusionService)
        {
            _fusionService = fusionService;
        }
        public async Task OnGetAsync()
        {
            FusedDisputes = await _fusionService.FuseDisputesAsync();
        }
    }
}
