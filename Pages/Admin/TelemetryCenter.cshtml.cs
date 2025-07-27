using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Services.Admin;
using MVP_Core.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MVP_Core.Pages.Admin
{
    public class TelemetryCenterModel : PageModel
    {
        private readonly UptimeAnalyzerService _analyzer;
        public TelemetryCenterModel(UptimeAnalyzerService analyzer)
        {
            _analyzer = analyzer;
        }

        public List<TechnicianSessionTelemetry> Telemetries { get; set; } = new();
        [BindProperty(SupportsGet = true)] public int? FilterTechnicianId { get; set; }
        [BindProperty(SupportsGet = true)] public string? FilterSessionId { get; set; }
        [BindProperty(SupportsGet = true)] public string? FilterRegion { get; set; }

        public async Task OnGetAsync()
        {
            Telemetries = await _analyzer.AnalyzeUptimeAsync(FilterTechnicianId, FilterRegion);
            if (!string.IsNullOrEmpty(FilterSessionId))
                Telemetries = Telemetries.FindAll(t => t.SessionId == FilterSessionId);
        }
    }
}