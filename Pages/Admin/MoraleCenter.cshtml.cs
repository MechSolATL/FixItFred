using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Data.Models;
using Services.Admin;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pages.Admin
{
    public class MoraleCenterModel : PageModel
    {
        private readonly TechnicianMoraleService _moraleService; // Sprint 79.3: CS8618/CS860X warning cleanup
        public MoraleCenterModel(TechnicianMoraleService moraleService)
        {
            _moraleService = moraleService ?? throw new ArgumentNullException(nameof(moraleService)); // Sprint 79.3: CS8618/CS860X warning cleanup
            MoraleLogs = new List<TechnicianMoraleLog>(); // Sprint 79.3: CS8618/CS860X warning cleanup
            Notes = string.Empty; // Sprint 79.3: CS8618/CS860X warning cleanup
        }
        [BindProperty]
        public int TechnicianId { get; set; } // Sprint 79.3: CS8618/CS860X warning cleanup
        [BindProperty]
        public int MoraleScore { get; set; } // Sprint 79.3: CS8618/CS860X warning cleanup
        [BindProperty]
        public string Notes { get; set; } = string.Empty; // Sprint 79.3: CS8618/CS860X warning cleanup
        [BindProperty]
        public int TrustImpact { get; set; } // Sprint 79.3: CS8618/CS860X warning cleanup
        public List<TechnicianMoraleLog> MoraleLogs { get; set; } = new List<TechnicianMoraleLog>(); // Sprint 79.3: CS8618/CS860X warning cleanup
        public async Task OnGetAsync()
        {
            MoraleLogs = TechnicianId > 0 ? await _moraleService.GetMoraleLogsAsync(TechnicianId) : new List<TechnicianMoraleLog>(); // Sprint 79.3: CS8618/CS860X warning cleanup
        }
        public async Task<IActionResult> OnPostFilterAsync()
        {
            MoraleLogs = TechnicianId > 0 ? await _moraleService.GetMoraleLogsAsync(TechnicianId) : new List<TechnicianMoraleLog>(); // Sprint 79.3: CS8618/CS860X warning cleanup
            return Page();
        }
        public async Task<IActionResult> OnPostAddMoraleAsync()
        {
            await _moraleService.AddMoraleEntryAsync(TechnicianId, MoraleScore, Notes, TrustImpact); // Sprint 79.3: CS8618/CS860X warning cleanup
            MoraleLogs = await _moraleService.GetMoraleLogsAsync(TechnicianId); // Sprint 79.3: CS8618/CS860X warning cleanup
            return Page();
        }
    }
}
