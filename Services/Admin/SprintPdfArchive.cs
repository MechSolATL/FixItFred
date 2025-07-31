// Sprint 47.3 – FixItFred Mission Package: Sprint PDF Archive Utility
using System;
using System.IO;
using System.Threading.Tasks;
using Data.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Services.Admin
{
    public static class SprintPdfArchive
    {
        public static async Task GenerateSprintChangeLogPdfAsync(string changelogText, string outputPath, string sprintRange, string author)
        {
            var now = DateTime.UtcNow;
            var doc = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(40);
                    page.Header().Text($"Sprint {sprintRange} ChangeLog Summary").FontSize(20).Bold();
                    page.Content().Column(col =>
                    {
                        col.Item().Text($"Generated: {now:u}").FontSize(10).Italic();
                        col.Item().Text($"Author: {author}").FontSize(10).Italic();
                        col.Item().PaddingVertical(10).LineHorizontal(1);
                        col.Item().Text(changelogText).FontSize(12);
                    });
                    page.Footer().AlignCenter().Text(x => x.Span($"© {now:yyyy} NovaOps | Sprint Archive").FontSize(9));
                });
            });
            Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
            using var ms = new MemoryStream();
            doc.GeneratePdf(ms);
            ms.Position = 0;
            using var fs = new FileStream(outputPath, FileMode.Create, FileAccess.Write);
            await ms.CopyToAsync(fs);
        }

        // Sprint 54.1: Generate branded estimate PDF
        public static async Task GenerateBrandedEstimatePdfAsync(
            BillingInvoiceRecord invoice,
            string outputPath,
            string logoPath,
            string contactInfo,
            string disclaimers,
            string terms,
            string serviceType,
            string[] lineItems,
            string? customerSignaturePath = null)
        {
            var now = DateTime.UtcNow;
            var doc = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(40);
                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("Estimate").FontSize(22).Bold();
                            col.Item().Text($"Date: {now:yyyy-MM-dd}").FontSize(10);
                            col.Item().Text(contactInfo).FontSize(10);
                        });
                        row.ConstantItem(120).Image(logoPath);
                    });
                    page.Content().Column(col =>
                    {
                        col.Item().Text($"Customer: {invoice.CustomerName}").FontSize(12);
                        col.Item().Text($"Email: {invoice.CustomerEmail}").FontSize(10);
                        col.Item().Text($"Service Type: {serviceType}").FontSize(11).Bold();
                        col.Item().PaddingVertical(8).LineHorizontal(1);
                        col.Item().Text("Line Items:").FontSize(11).Bold();
                        foreach (var item in lineItems)
                            col.Item().Text("- " + item).FontSize(10);
                        col.Item().PaddingVertical(8).LineHorizontal(1);
                        col.Item().Text("Terms:").FontSize(10).Bold();
                        col.Item().Text(terms).FontSize(9);
                        col.Item().PaddingVertical(8).LineHorizontal(1);
                        col.Item().Text("Disclaimers:").FontSize(10).Bold();
                        col.Item().Text(disclaimers).FontSize(9);
                        if (!string.IsNullOrEmpty(customerSignaturePath))
                        {
                            col.Item().PaddingVertical(8).Text("Customer Signature:").FontSize(10).Bold();
                            col.Item().Image(customerSignaturePath, ImageScaling.FitWidth);
                        }
                    });
                    page.Footer().AlignCenter().Text(x => x.Span($"© {now:yyyy} NovaOps | Estimate").FontSize(9));
                });
            });
            Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
            using var ms = new MemoryStream();
            doc.GeneratePdf(ms);
            ms.Position = 0;
            using var fs = new FileStream(outputPath, FileMode.Create, FileAccess.Write);
            await ms.CopyToAsync(fs);
        }
    }
}
