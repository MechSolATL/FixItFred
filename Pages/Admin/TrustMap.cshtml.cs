// Sprint 84.8 — TrustMap UI Scaffold
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Services.Admin;
using MVP_Core.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MVP_Core.Pages.Admin
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
        public List<TechnicianDropAlertDto> DropAlerts { get; set; } = new();

        public async Task OnGetAsync()
        {
            HeatScores = await _analyticsService.GetHeatScoreMapData();
            DropAlerts = await _analyticsService.GetRecentDropAlerts();
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
    }
    public class TechnicianDropAlertDto
    {
        public int TechnicianId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int PreviousScore { get; set; }
        public int CurrentScore { get; set; }
        public string City { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        public string DropReason { get; set; } = string.Empty;
    }
}
