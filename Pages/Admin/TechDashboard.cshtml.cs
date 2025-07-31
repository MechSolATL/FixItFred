using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Data.Models;
using Data;
using Services.Admin;

namespace Pages.Admin
{
    public class TechDashboardModel : PageModel
    {
        private readonly TechnicianInsightService _insightService;
        private readonly ApplicationDbContext _db;
        public TechDashboardModel(TechnicianInsightService insightService, ApplicationDbContext db)
        {
            _insightService = insightService;
            _db = db;
        }

        public List<TechnicianKPIViewModel> KPIs { get; set; } = new();
        public List<TechnicianInsightLog> InsightLogs { get; set; } = new();
        [BindProperty(SupportsGet = true)]
        public int? FilterTechnicianId { get; set; }
        [BindProperty(SupportsGet = true)]
        public string FilterInsightType { get; set; } = string.Empty;
        [BindProperty]
        public string NewInsightType { get; set; } = string.Empty;
        [BindProperty]
        public string NewInsightDetail { get; set; } = string.Empty;

        public async Task OnGetAsync()
        {
            KPIs = await _insightService.GetTechnicianKPIsAsync();
            var logsQuery = _db.TechnicianInsightLogs.AsQueryable();
            if (FilterTechnicianId.HasValue)
                logsQuery = logsQuery.Where(l => l.TechnicianId == FilterTechnicianId.Value);
            if (!string.IsNullOrWhiteSpace(FilterInsightType))
                logsQuery = logsQuery.Where(l => l.InsightType.Contains(FilterInsightType));
            InsightLogs = await logsQuery.OrderByDescending(l => l.LoggedAt).Take(50).ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (FilterTechnicianId.HasValue && !string.IsNullOrWhiteSpace(NewInsightType) && !string.IsNullOrWhiteSpace(NewInsightDetail))
            {
                await _insightService.LogInsightAsync(FilterTechnicianId.Value, NewInsightType, NewInsightDetail);
            }
            return RedirectToPage(new { TechnicianId = FilterTechnicianId, InsightType = FilterInsightType });
        }
    }
}
