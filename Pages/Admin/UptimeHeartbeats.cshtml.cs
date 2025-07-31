using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Data.Models;
using Services.Admin;

namespace Pages.Admin
{
    public class UptimeHeartbeatsModel : PageModel
    {
        private readonly UptimeAnalyzerService _analyzer;
        public UptimeHeartbeatsModel(UptimeAnalyzerService analyzer)
        {
            _analyzer = analyzer;
        }

        public List<UptimeHeartbeatLog> Heartbeats { get; set; } = new();
        [BindProperty(SupportsGet = true)] public int? FilterTechnicianId { get; set; }
        [BindProperty(SupportsGet = true)] public string? FilterSessionId { get; set; }
        [BindProperty(SupportsGet = true)] public string? FilterRegion { get; set; }

        public async Task OnGetAsync()
        {
            Heartbeats = await _analyzer.GetHeatSignatureMap(FilterRegion);
            if (FilterTechnicianId.HasValue)
                Heartbeats = Heartbeats.FindAll(h => h.TechnicianId == FilterTechnicianId.Value);
            if (!string.IsNullOrEmpty(FilterSessionId))
                Heartbeats = Heartbeats.FindAll(h => h.SessionId == FilterSessionId);
        }

        public async Task<IActionResult> OnPostExportCsvAsync()
        {
            Heartbeats = await _analyzer.GetHeatSignatureMap(FilterRegion);
            if (FilterTechnicianId.HasValue)
                Heartbeats = Heartbeats.FindAll(h => h.TechnicianId == FilterTechnicianId.Value);
            if (!string.IsNullOrEmpty(FilterSessionId))
                Heartbeats = Heartbeats.FindAll(h => h.SessionId == FilterSessionId);
            var sb = new StringBuilder();
            sb.AppendLine("TechnicianId,HeartbeatAt,IsActive,BatteryLevel,NetworkType,SessionId,Region,GeoConfidence,LogSource");
            foreach (var log in Heartbeats)
            {
                sb.AppendLine($"{log.TechnicianId},{log.HeartbeatAt},{log.IsActive},{log.BatteryLevel},{log.NetworkType},{log.SessionId},{log.Region},{log.GeoConfidence},{log.LogSource}");
            }
            return File(Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", "UptimeHeartbeats.csv");
        }
    }
}