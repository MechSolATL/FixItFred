using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using Services.Admin;

namespace Pages.Admin
{
    public class InvestorReportsModel : PageModel
    {
        private readonly InvestorAnalyticsService _analyticsService;
        public InvestorReportsModel(InvestorAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        [BindProperty(SupportsGet = true)] public DateTime? Start { get; set; }
        [BindProperty(SupportsGet = true)] public DateTime? End { get; set; }
        [BindProperty(SupportsGet = true)] public string? Zone { get; set; }
        [BindProperty(SupportsGet = true)] public int? TechnicianId { get; set; }
        [BindProperty(SupportsGet = true)] public string? ServiceType { get; set; }

        public List<InvestorAnalyticsService.RevenueSummary> RevenueByMonth { get; set; } = new();
        public double SlaCompletionRate { get; set; }
        public List<InvestorAnalyticsService.ReviewSummary> AverageReviewByServiceType { get; set; } = new();
        public List<InvestorAnalyticsService.TechnicianPayoutSummary> TechnicianLeaderboard { get; set; } = new();

        public async Task OnGetAsync()
        {
            RevenueByMonth = await _analyticsService.GetRevenueByMonthAsync(Start, End, ServiceType);
            SlaCompletionRate = await _analyticsService.GetSlaCompletionRateAsync(Start, End, Zone, ServiceType);
            AverageReviewByServiceType = await _analyticsService.GetAverageReviewByServiceTypeAsync(Start, End);
            TechnicianLeaderboard = await _analyticsService.GetTechnicianPayoutSummaryAsync(Start, End);
        }

        public async Task<IActionResult> OnPostExportPdfAsync()
        {
            var report = new InvestorAnalyticsReport
            {
                ExecutiveSummary = $"Investor Analytics Report for {Start:yyyy-MM-dd} to {End:yyyy-MM-dd}",
                RevenueByMonth = await _analyticsService.GetRevenueByMonthAsync(Start, End, ServiceType),
                SlaCompletionRate = await _analyticsService.GetSlaCompletionRateAsync(Start, End, Zone, ServiceType),
                AverageReviewByServiceType = await _analyticsService.GetAverageReviewByServiceTypeAsync(Start, End),
                TechnicianPayoutSummary = await _analyticsService.GetTechnicianPayoutSummaryAsync(Start, End),
                JobFunnel = await _analyticsService.GetJobFunnelSummaryAsync(Start, End, ServiceType)
            };
            var outputDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "exports");
            var sprintTag = $"{DateTime.UtcNow:yyyyMMdd}";
            await InvestorExportPipeline.GenerateInvestorPackAsync(report, outputDir, sprintTag);
            TempData["PdfExported"] = $"Investor Pitch PDF generated: exports/InvestorPack_{sprintTag}.pdf";
            return RedirectToPage();
        }
    }
}
