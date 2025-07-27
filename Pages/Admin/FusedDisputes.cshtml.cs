using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Services.Admin;
using MVP_Core.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MVP_Core.Pages.Admin
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
