using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Data.Models;
using MVP_Core.Services.Admin;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MVP_Core.Pages.Admin
{
    public class EscalationCenterModel : PageModel
    {
        private readonly TechnicianEscalationService _service;
        public EscalationCenterModel(TechnicianEscalationService service)
        {
            _service = service;
        }

        [BindProperty(SupportsGet = true)]
        public int? FilterTechnicianId { get; set; }
        [BindProperty(SupportsGet = true)]
        public bool? FilterResolved { get; set; }
        public List<TechnicianEscalationLog> Escalations { get; set; } = new();

        public async Task OnGetAsync()
        {
            Escalations = await _service.GetEscalationsAsync(FilterTechnicianId, FilterResolved);
        }

        public async Task<IActionResult> OnPostAsync(int EscalationId, string ResolutionNotes, int? TrustImpactTier)
        {
            await _service.ResolveEscalationAsync(EscalationId, ResolutionNotes, TrustImpactTier);
            return RedirectToPage();
        }
    }
}
