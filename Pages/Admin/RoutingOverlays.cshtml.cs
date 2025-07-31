using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using Data.Models;
using Services.Admin;

namespace Pages.Admin
{
    public class RoutingOverlaysModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly RoutingOverlayService _overlayService;
        public RoutingOverlaysModel(ApplicationDbContext db)
        {
            _db = db;
            _overlayService = new RoutingOverlayService(db);
        }

        public List<RoutingOverlayRegion> OverlayRegions { get; set; } = new();

        public async Task OnGetAsync()
        {
            OverlayRegions = await _db.RoutingOverlayRegions.ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync(string RegionName, string GeoBoundaryJson, string PreferredTechIds, int ZonePriority)
        {
            await _overlayService.CreateOverlay(RegionName, GeoBoundaryJson, PreferredTechIds, ZonePriority);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostUpdatePriorityAsync(int RegionId, int NewPriority)
        {
            await _overlayService.UpdateRegionPriority(RegionId, NewPriority);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostAssignTechsAsync(int RegionId, string NewTechIds)
        {
            await _overlayService.AssignTechnicianRouting(RegionId, NewTechIds);
            return RedirectToPage();
        }
    }
}
