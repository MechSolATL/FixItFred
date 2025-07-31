using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Data.Models;
using Services.Admin;

namespace Pages.Admin
{
    public class EmployeePulseModel : PageModel
    {
        private readonly SanityPulseService _pulseService;
        public EmployeePulseModel(SanityPulseService pulseService)
        {
            _pulseService = pulseService;
            PulseLogs = new List<WellBeingPulseLog>();
        }
        public List<WellBeingPulseLog> PulseLogs { get; set; }
        [BindProperty(SupportsGet = true)]
        public bool ShowUnresolved { get; set; }
        public async Task OnGetAsync()
        {
            PulseLogs = ShowUnresolved
                ? await _pulseService.GetUnresolvedAsync()
                : await _pulseService.GetRecentLogsAsync();
        }
        public async Task<IActionResult> OnPostResolveAsync(int id, string? managerNote)
        {
            await _pulseService.ResolveLogAsync(id, managerNote);
            return RedirectToPage();
        }
    }
}
