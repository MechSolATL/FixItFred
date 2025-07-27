using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Services.Admin;
using MVP_Core.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MVP_Core.Pages.Admin
{
    public class SanityShieldModel : PageModel
    {
        private readonly SanityShieldService _sanityShieldService;
        public SanityShieldModel(SanityShieldService sanityShieldService)
        {
            _sanityShieldService = sanityShieldService;
        }
        [BindProperty]
        public int TechnicianId { get; set; }
        public List<TechnicianSanityLog> SanityLogs { get; set; } = new();
        public async Task OnGetAsync()
        {
            SanityLogs = await _sanityShieldService.GetSanityLogsAsync(TechnicianId);
        }
        public async Task<IActionResult> OnPostAnalyzeAsync()
        {
            await _sanityShieldService.AnalyzeAndShieldAsync(TechnicianId);
            SanityLogs = await _sanityShieldService.GetSanityLogsAsync(TechnicianId);
            return Page();
        }
    }
}
