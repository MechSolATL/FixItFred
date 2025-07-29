using System;
using System.IO;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace MVP_Core.Services.Admin
{
    // Sprint 83.4: PDF composer for README onboarding packet
    public static class PDFPacketComposer
    {
        public static void GenerateReadmePdf(string rawReadme, string outputPath, DateTime certificationDate, string hash, bool includeLLMQuote = true)
        {
            var watermark = "P.R.O.S. Certified • Officially Recognized as a Certified Service Provider of Their Craftsmanship";
            var mission = "Powered by Patch • Built on the PROS Street Creed";
            var company = "Mechanical Solutions Atlanta | Service-Atlanta.com | (c) 2025";
            var llmQuote = "We’re building a self-learning technician platform where every job teaches the next. It’s powered by AI, guided by experience, and built on the values that define real pros.";
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
                    page.Footer().Column(footerCol =>
                    {
                        footerCol.Item().AlignCenter().Text($"Certified: {certificationDate:yyyy-MM-dd} | Hash: {hash}").FontSize(9);
                        footerCol.Item().AlignCenter().Text(mission).FontSize(9).Italic();
                        footerCol.Item().AlignCenter().Text(company).FontSize(9);
                        if (includeLLMQuote)
                            footerCol.Item().AlignCenter().Text(llmQuote).FontSize(8).Italic();
                    });
                });
            });
            Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
            doc.GeneratePdf(outputPath);
        }
    }

    // Sprint 90.0: PDF composer for Service-Atlanta Revitalize MasterPlan blueprint
    public static class BlueprintPdfComposer
    {
        public static void GenerateBlueprintPdf(string markdownContent, string outputPath, bool includeLLMQuote = true)
        {
            var mission = "Powered by Patch • Built on the PROS Street Creed";
            var company = "Mechanical Solutions Atlanta | Service-Atlanta.com | (c) 2025";
            var llmQuote = "We’re building a self-learning technician platform where every job teaches the next. It’s powered by AI, guided by experience, and built on the values that define real pros.";
            var doc = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(40);
                    page.Header().Text("Service-Atlanta Revitalize Master Execution Blueprint").FontSize(18).Bold();
                    page.Content().Column(col =>
                    {
                        col.Item().Text(markdownContent).FontSize(10);
                        col.Item().PaddingVertical(10).LineHorizontal(1);
                        col.Item().Text("© 2025 NovaOps | Service-Atlanta Revitalize Initiative").FontSize(9).Italic();
                    });
                    page.Footer().Column(footerCol =>
                    {
                        footerCol.Item().AlignCenter().Text($"Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm} UTC").FontSize(8);
                        footerCol.Item().AlignCenter().Text(mission).FontSize(9).Italic();
                        footerCol.Item().AlignCenter().Text(company).FontSize(9);
                        if (includeLLMQuote)
                            footerCol.Item().AlignCenter().Text(llmQuote).FontSize(8).Italic();
                    });
                });
            });
            Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
            doc.GeneratePdf(outputPath);
        }
    }
}
