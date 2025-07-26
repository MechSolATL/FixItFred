using System;
using System.IO;
using System.Threading.Tasks;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MVP_Core.Services.Admin
{
    public static class InvestorExportPipeline
    {
        public static async Task GenerateInvestorPackAsync(string summary, string architectureDiagramPath, string featureMatrix, string revenueModel, string pitchPack, string outputDir, string sprintTag)
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
                        col.Item().Text("Sprint Summary").FontSize(16).Bold();
                        col.Item().Text(summary).FontSize(12);
                        col.Item().PaddingVertical(10).LineHorizontal(1);
                        col.Item().Text("System Architecture").FontSize(16).Bold();
                        if (!string.IsNullOrEmpty(architectureDiagramPath) && File.Exists(architectureDiagramPath))
                            col.Item().Image(architectureDiagramPath, ImageScaling.FitWidth);
                        col.Item().PaddingVertical(10).LineHorizontal(1);
                        col.Item().Text("Feature Matrix").FontSize(16).Bold();
                        col.Item().Text(featureMatrix).FontSize(12);
                        col.Item().PaddingVertical(10).LineHorizontal(1);
                        col.Item().Text("Revenue Model & Usage Cases").FontSize(16).Bold();
                        col.Item().Text(revenueModel).FontSize(12);
                        col.Item().PaddingVertical(10).LineHorizontal(1);
                        col.Item().Text("VC Pitch Pack").FontSize(16).Bold();
                        col.Item().Text(pitchPack).FontSize(12);
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
}
