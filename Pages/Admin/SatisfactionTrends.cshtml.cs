using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOs.Reports;
using Services.Admin;

namespace Pages.Admin
{
    [Authorize(Roles = "Admin")]
    // Sprint 43 – Satisfaction Analytics
    public class SatisfactionTrendsModel : PageModel
    {
        private readonly DispatcherService _dispatcherService;
        public SatisfactionTrendsModel(DispatcherService dispatcherService)
        {
            _dispatcherService = dispatcherService;
        }
        [BindProperty(SupportsGet = true)] public string? Technician { get; set; }
        [BindProperty(SupportsGet = true)] public string? ServiceType { get; set; }
        [BindProperty(SupportsGet = true)] public string? Outcome { get; set; }
        [BindProperty(SupportsGet = true)] public string? GroupBy { get; set; } = "Technician";
        [BindProperty(SupportsGet = true)] public DateTime? Start { get; set; }
        [BindProperty(SupportsGet = true)] public DateTime? End { get; set; }
        public List<SatisfactionAnalyticsDto> Analytics { get; set; } = new();
        public async Task OnGetAsync()
        {
            Analytics = await _dispatcherService.GetSatisfactionAnalyticsAsync(Start, End, Technician, ServiceType, Outcome, GroupBy ?? "Technician");
        }
        public async Task<IActionResult> OnGetExportAsync()
        {
            var data = await _dispatcherService.GetSatisfactionAnalyticsAsync(Start, End, Technician, ServiceType, Outcome, GroupBy ?? "Technician");
            var csv = new StringBuilder();
            csv.AppendLine("Group,GroupType,AverageScore,Count");
            foreach (var row in data)
                csv.AppendLine($"{row.GroupKey},{row.GroupType},{row.AverageScore:0.00},{row.Count}");
            var bytes = Encoding.UTF8.GetBytes(csv.ToString());
            return File(bytes, "text/csv", "satisfaction-trends.csv");
        }
        public async Task<IActionResult> OnGetChartDataAsync()
        {
            var data = await _dispatcherService.GetSatisfactionAnalyticsAsync(Start, End, Technician, ServiceType, Outcome, GroupBy ?? "Technician");
            return new JsonResult(data);
        }
    }
}
// End Sprint 43 – Satisfaction Analytics
