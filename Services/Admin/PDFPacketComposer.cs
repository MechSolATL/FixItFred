using System;
using System.IO;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace MVP_Core.Services.Admin
{
    // Sprint 83.4: PDF composer for README onboarding packet
    public static class PDFPacketComposer
    {
        public static void GenerateReadmePdf(string rawReadme, string outputPath, DateTime certificationDate, string hash)
        {
            var watermark = "P.R.O.S. Certified — Officially Recognized as a Certified Service Provider of Their Craftsmanship";
            var doc = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(40);
                    page.Header().Text("Onboarding README Packet").FontSize(18).Bold();
                    page.Content().Column(col =>
                    {
                        col.Item().Text(rawReadme).FontSize(10);
                        col.Item().PaddingVertical(10).LineHorizontal(1);
                        col.Item().Text(watermark).FontSize(12).Bold().FontColor("#007bff");
                    });
                    page.Footer().AlignCenter().Text($"Certified: {certificationDate:yyyy-MM-dd} | Hash: {hash}").FontSize(9);
                });
            });
            Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
            doc.GeneratePdf(outputPath);
        }
    }
}
