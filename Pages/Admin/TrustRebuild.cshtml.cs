// Sprint 85.1 — Trust Rebuild Suggestion Engine Core
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using MVP_Core.Data;
using MVP_Core.Data.Models;
using MVP_Core.Models;
using MVP_Core.Services.Admin;
using System.Collections.Generic;
using System.Linq;

namespace MVP_Core.Pages.Admin
{
    // Sprint 85.1 — Trust Rebuild Suggestions UI
    public class TrustRebuildModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly TrustRebuildAdvisorService _advisorService;
        public TrustRebuildModel(ApplicationDbContext db, TrustRebuildAdvisorService advisorService)
        {
            _db = db;
            _advisorService = advisorService;
        }
        [BindProperty(SupportsGet = true, Name = "technicianId")]
        public int? TechnicianId { get; set; }
        public List<MVP_Core.Data.Models.Technician> Technicians { get; set; } = new();
        public int SelectedTechnicianId => TechnicianId ?? 0;
        public List<TrustRebuildSuggestion> Suggestions { get; set; } = new();
        public void OnGet()
        {
            Technicians = _db.Technicians.OrderBy(t => t.FullName).ToList();
            if (TechnicianId.HasValue)
            {
                Suggestions = _advisorService.GetSuggestionsForTechnician(TechnicianId.Value);
            }
        }
    }
}
