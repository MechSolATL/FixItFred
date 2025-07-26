// Sprint 47.3 – FixItFred Mission Package: Sprint PDF Archive Utility
using System;
using System.IO;
using System.Threading.Tasks;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MVP_Core.Services.Admin
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
    }
}
