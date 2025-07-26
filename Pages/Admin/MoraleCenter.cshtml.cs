using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Data.Models;
using MVP_Core.Services.Admin;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MVP_Core.Pages.Admin
{
    public class MoraleCenterModel : PageModel
    {
        private readonly TechnicianMoraleService _moraleService;
        public MoraleCenterModel(TechnicianMoraleService moraleService)
        {
            _moraleService = moraleService;
            MoraleLogs = new List<TechnicianMoraleLog>();
        }
        [BindProperty]
        public int TechnicianId { get; set; }
        [BindProperty]
        public int MoraleScore { get; set; }
        [BindProperty]
        public string Notes { get; set; }
        [BindProperty]
        public int TrustImpact { get; set; }
        public List<TechnicianMoraleLog> MoraleLogs { get; set; }
        public async Task OnGetAsync()
        {
            MoraleLogs = TechnicianId > 0 ? await _moraleService.GetMoraleLogsAsync(TechnicianId) : new List<TechnicianMoraleLog>();
        }
        public async Task<IActionResult> OnPostFilterAsync()
        {
            MoraleLogs = TechnicianId > 0 ? await _moraleService.GetMoraleLogsAsync(TechnicianId) : new List<TechnicianMoraleLog>();
            return Page();
        }
        public async Task<IActionResult> OnPostAddMoraleAsync()
        {
            await _moraleService.AddMoraleEntryAsync(TechnicianId, MoraleScore, Notes, TrustImpact);
            MoraleLogs = await _moraleService.GetMoraleLogsAsync(TechnicianId);
            return Page();
        }
    }
}
