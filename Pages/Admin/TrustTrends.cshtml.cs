using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Text;
using Services.Admin;
using Data;

namespace Pages.Admin
{
    // Sprint 85.0 — Trust Trends Chart Logic + Filters
    public class TrustTrendsModel : PageModel
    {
        private readonly TechnicianTrustAnalyticsService _analyticsService;
        private readonly ApplicationDbContext _db;
        public TrustTrendsModel(TechnicianTrustAnalyticsService analyticsService, ApplicationDbContext db)
        {
            _analyticsService = analyticsService;
            _db = db;
        }
        [BindProperty(SupportsGet = true)] public int? TechnicianId { get; set; }
        [BindProperty(SupportsGet = true)] public int DateRangeIndex { get; set; } = 29;
        public List<Data.Models.Technician> Technicians { get; set; } = new();
        public DateTime StartDate => DateTime.UtcNow.Date.AddDays(-DateRangeIndex);
        public DateTime EndDate => DateTime.UtcNow.Date;
        public List<TechnicianTrendPoint> TrendData { get; set; } = new();

        public async Task OnGetAsync()
        {
            Technicians = _db.Technicians.OrderBy(t => t.FullName).ToList();
            TrendData = await _analyticsService.GetTechnicianTrustTrends(StartDate, EndDate, TechnicianId);
        }

        public async Task<IActionResult> OnGetChartDataAsync(int? technicianId, int dateRangeIndex)
        {
            var start = DateTime.UtcNow.Date.AddDays(-dateRangeIndex);
            var end = DateTime.UtcNow.Date;
            var data = await _analyticsService.GetTechnicianTrustTrends(start, end, technicianId);
            return new JsonResult(data);
        }

        public async Task<IActionResult> OnGetExportAsync(int? technicianId, int dateRangeIndex)
        {
            var start = DateTime.UtcNow.Date.AddDays(-dateRangeIndex);
            var end = DateTime.UtcNow.Date;
            var data = await _analyticsService.GetTechnicianTrustTrends(start, end, technicianId);
            var csv = new StringBuilder();
            csv.AppendLine("TechnicianId,Date,HeatScore");
            foreach (var row in data)
                csv.AppendLine($"{row.TechnicianId},{row.Date:yyyy-MM-dd},{row.HeatScore}");
            var bytes = Encoding.UTF8.GetBytes(csv.ToString());
            return File(bytes, "text/csv", "trustscore-trends.csv");
        }
    }

    // Sprint 85.0 — Trust Trends Chart Logic + Filters
    public class TechnicianTrendPoint
    {
        public int TechnicianId { get; set; }
        public string TechnicianName { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public int HeatScore { get; set; }
    }
}
// Sprint 85.0 — Trust Trends Chart Logic + Filters
