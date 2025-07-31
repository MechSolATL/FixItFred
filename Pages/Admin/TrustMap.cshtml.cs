// Sprint 84.8 — TrustMap UI Scaffold
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Services.Admin;

namespace Pages.Admin
{
    // Sprint 84.8 — Technician Heat Score + Map Overlay
    public class TrustMapModel : PageModel
    {
        private readonly TechnicianTrustAnalyticsService _analyticsService;
        public TrustMapModel(TechnicianTrustAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        public List<TechnicianHeatScoreDto> HeatScores { get; set; } = new();

        public async Task OnGetAsync()
        {
            HeatScores = await _analyticsService.GetHeatScoreMapData();
        }

        // Sprint 84.8 Phase 2 — TrustMap Interactivity + GeoCluster
        public async Task<IActionResult> OnGetTrustMapDataAsync()
        {
            var data = await _analyticsService.GetHeatScoreMapData();
            // Optionally enrich with city/region summary
            var grouped = data.GroupBy(t => t.City ?? t.ZipCode ?? "Unknown")
                .Select(g => new {
                    City = g.Key,
                    Technicians = g.ToList(),
                    TechnicianCount = g.Count(),
                    AvgHeatScore = g.Average(t => t.HeatScore),
                    LastActivity = g.Max(t => t.TechnicianId) // Placeholder for real last activity
                });
            return new JsonResult(grouped);
        }
    }

    // Sprint 84.8 — Technician Heat Score + Map Overlay
    public class TechnicianHeatScoreDto
    {
        public int TechnicianId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int HeatScore { get; set; }
        public string City { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        public double? Latitude { get; set; } // For map plotting
        public double? Longitude { get; set; } // For map plotting
        public string? LastActivity { get; set; } // For hover summary
    }
}
