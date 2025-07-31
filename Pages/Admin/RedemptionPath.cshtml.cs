using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;
using Data.Models;
using Services.Admin;
using Data;

namespace Pages.Admin
{
    public class RedemptionPathModel : PageModel
    {
        private readonly TrustRedemptionService _redemptionService;
        private readonly ApplicationDbContext _db;
        public RedemptionPathModel(TrustRedemptionService redemptionService, ApplicationDbContext db)
        {
            _redemptionService = redemptionService;
            _db = db;
        }
        public List<RedemptionOpportunity> Opportunities { get; set; } = new();
        public List<Data.Models.Technician> Technicians { get; set; } = new();
        [BindProperty(SupportsGet = true)]
        public int? FilterTechnicianId { get; set; }
        [BindProperty(SupportsGet = true)]
        public string FilterStatus { get; set; } = string.Empty;
        [BindProperty]
        public int NewTechnicianId { get; set; }
        [BindProperty]
        public string NewOpportunityType { get; set; } = string.Empty;
        [BindProperty]
        public string? NewDescription { get; set; }
        [BindProperty]
        public int NewPointsRequired { get; set; }
        [BindProperty]
        public int ResolveOpportunityId { get; set; }
        [BindProperty]
        public string? ResolutionNotes { get; set; }
        public async Task OnGetAsync()
        {
            Technicians = await _db.Technicians.OrderBy(t => t.FullName).ToListAsync();
            var query = _db.RedemptionOpportunities.AsQueryable();
            if (FilterTechnicianId.HasValue)
                query = query.Where(o => o.TechnicianId == FilterTechnicianId.Value);
            if (!string.IsNullOrWhiteSpace(FilterStatus))
                query = query.Where(o => o.Status == FilterStatus);
            Opportunities = await query.OrderByDescending(o => o.CreatedAt).ToListAsync();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (ResolveOpportunityId > 0 && !string.IsNullOrWhiteSpace(ResolutionNotes))
            {
                await _redemptionService.ResolveOpportunityAsync(ResolveOpportunityId, ResolutionNotes);
            }
            else if (NewTechnicianId > 0 && !string.IsNullOrWhiteSpace(NewOpportunityType))
            {
                await _redemptionService.LogOpportunityAsync(NewTechnicianId, NewOpportunityType, NewDescription, NewPointsRequired);
            }
            return RedirectToPage(new { TechnicianId = FilterTechnicianId, Status = FilterStatus });
        }
    }
}
