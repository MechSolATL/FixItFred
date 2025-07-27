using MVP_Core.Data;
using MVP_Core.Data.Models;
using MVP_Core.Services.Admin;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVP_Core.Pages.Admin
{
    public class HeatmapCenterModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly HeatmapAnalyticsService _heatmapService;
        private readonly RoutingOverlayService _overlayService;
        public HeatmapCenterModel(ApplicationDbContext db)
        {
            _db = db;
            _heatmapService = new HeatmapAnalyticsService(db);
            _overlayService = new RoutingOverlayService(db);
        }

        public int? TechId { get; set; }
        public string? ServiceType { get; set; }
        public int? JobCount { get; set; }
        public List<RoutingOverlayRegion> OverlayRegions { get; set; } = new();
        public List<string> CoverageGaps { get; set; } = new();

        public async Task OnGetAsync(int? techId, string? serviceType, int? jobCount)
        {
            TechId = techId;
            ServiceType = serviceType;
            JobCount = jobCount;
            OverlayRegions = await _db.RoutingOverlayRegions.OrderByDescending(r => r.ZonePriority).ToListAsync();
            CoverageGaps = await _heatmapService.DetectCoverageGaps();
        }
    }
}
