using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Data.Models;
using Services.Admin;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pages.Admin
{
    public class TrustRecoveryModel : PageModel
    {
        private readonly RedemptionService _redemptionService;
        public TrustRecoveryModel(RedemptionService redemptionService)
        {
            _redemptionService = redemptionService;
            RedemptionLogs = new List<TechnicianRedemptionLog>();
        }
        public List<TechnicianRedemptionLog> RedemptionLogs { get; set; }
        public async Task OnGetAsync()
        {
            RedemptionLogs = await _redemptionService.GetRedemptionEntriesAsync();
        }
    }
}
