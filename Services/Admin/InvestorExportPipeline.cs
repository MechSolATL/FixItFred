using System;
using System.IO;
using System.Threading.Tasks;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Collections.Generic;

namespace Services.Admin
{
    public static class InvestorExportPipeline
    {
        public static async Task GenerateInvestorPackAsync(
            InvestorAnalyticsReport report,
            string outputDir,
            string sprintTag)
        {
            var now = DateTime.UtcNow;
            Directory.CreateDirectory(outputDir);
            var doc = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(40);
                    page.Header().Text($"Investor Pack – {sprintTag}").FontSize(22).Bold();
                    page.Content().Column(col =>
                    {
                        col.Item().Text($"Generated: {now:u}").FontSize(10).Italic();
                        col.Item().PaddingVertical(10).LineHorizontal(1);
                        col.Item().Text("Executive Summary").FontSize(16).Bold();
                        col.Item().Text(report.ExecutiveSummary).FontSize(12);
                        col.Item().PaddingVertical(10).LineHorizontal(1);
                        col.Item().Text("Revenue by Month").FontSize(16).Bold();
                        foreach (var rev in report.RevenueByMonth)
                            col.Item().Text($"{rev.Year}-{rev.Month:00}: ${rev.TotalRevenue:N2}").FontSize(12);
                        col.Item().PaddingVertical(10).LineHorizontal(1);
                        col.Item().Text($"SLA Completion: {report.SlaCompletionRate:N2}%").FontSize(12);
                        col.Item().PaddingVertical(10).LineHorizontal(1);
                        col.Item().Text("Average Review by Service Type").FontSize(16).Bold();
                        foreach (var r in report.AverageReviewByServiceType)
                            col.Item().Text($"{r.ServiceType}: {r.AverageRating:N2} stars").FontSize(12);
                        col.Item().PaddingVertical(10).LineHorizontal(1);
                        col.Item().Text("Technician Payout & Bonus Summary").FontSize(16).Bold();
                        foreach (var t in report.TechnicianPayoutSummary)
                            col.Item().Text($"Tech #{t.TechnicianId}: Pay ${t.TotalPay:N2}, Commission ${t.Commission:N2}, Bonus ${t.TotalBonus:N2}").FontSize(12);
                        col.Item().PaddingVertical(10).LineHorizontal(1);
                        col.Item().Text("Job Volume & Conversion Funnel").FontSize(16).Bold();
                        col.Item().Text($"Total: {report.JobFunnel.Total}, Scheduled: {report.JobFunnel.Scheduled}, Completed: {report.JobFunnel.Completed}, Cancelled: {report.JobFunnel.Cancelled}").FontSize(12);
                    });
                    page.Footer().AlignCenter().Text(x => x.Span($"© {now:yyyy} NovaOps | Investor Export").FontSize(9));
                });
            });
            var pdfPath = Path.Combine(outputDir, $"InvestorPack_{sprintTag}.pdf");
            using var ms = new MemoryStream();
            doc.GeneratePdf(ms);
            ms.Position = 0;
            using var fs = new FileStream(pdfPath, FileMode.Create, FileAccess.Write);
            await ms.CopyToAsync(fs);
        }
    }

    // DTO for report
    public class InvestorAnalyticsReport
    {
        public string ExecutiveSummary { get; set; } = "";
        public List<InvestorAnalyticsService.RevenueSummary> RevenueByMonth { get; set; } = new();
        public double SlaCompletionRate { get; set; }
        public List<InvestorAnalyticsService.ReviewSummary> AverageReviewByServiceType { get; set; } = new();
        public List<InvestorAnalyticsService.TechnicianPayoutSummary> TechnicianPayoutSummary { get; set; } = new();
        public InvestorAnalyticsService.JobFunnelSummary JobFunnel { get; set; } = new();
    }
}
