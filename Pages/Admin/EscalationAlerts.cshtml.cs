using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Data.Models;
using Services.Admin;

namespace Pages.Admin
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
