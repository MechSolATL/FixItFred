using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Models;
using Services.Admin;

namespace Pages.Admin
{
    public class SlaAlertsModel : PageModel
    {
        private readonly SlaAlertService _slaService;
        public SlaAlertsModel(SlaAlertService slaService)
        {
            _slaService = slaService;
        }

        public List<SlaMissAlert> Alerts { get; set; } = new();

        public async Task OnGetAsync()
        {
            Alerts = await _slaService.GetUnresolvedViolations();
        }

        public async Task<IActionResult> OnPostResolveAsync(int alertId)
        {
            var alert = await _slaService.Db.SlaMissAlerts.FindAsync(alertId);
            if (alert != null)
            {
                alert.ResolutionStatus = "Resolved";
                await _slaService.Db.SaveChangesAsync();
            }
            return RedirectToPage();
        }
    }
}