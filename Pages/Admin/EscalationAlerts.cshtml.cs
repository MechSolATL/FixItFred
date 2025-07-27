using MVP_Core.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using MVP_Core.Services.Admin;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;

namespace MVP_Core.Pages.Admin
{
    public class EscalationAlertsModel : PageModel
    {
        private readonly AutoEscalationEngine _engine;
        public List<DepartmentDelayLog> Alerts { get; set; } = new();
        [BindProperty(SupportsGet = true)] public int ThresholdMinutes { get; set; } = 60;
        public EscalationAlertsModel(AutoEscalationEngine engine) { _engine = engine; }
        public async Task OnGetAsync()
        {
            Alerts = await _engine.GetOpenDelaysAsync(ThresholdMinutes);
        }
        public async Task<IActionResult> OnPostOverrideAsync(int delayId, int escalationLevel, string reason)
        {
            await _engine.EscalateDelayAsync(delayId, escalationLevel, reason);
            return RedirectToPage();
        }
    }
}
