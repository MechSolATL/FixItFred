using System;
using System.IO;

// Standalone console app for generating the PDF blueprint
namespace Services.Admin
{
    class BlueprintGen
    {
        static void Main(string[] args)
        {
            string mdPath = Path.Combine("Docs", "Blueprints", "Service-Atlanta-Revitalize-MasterPlan.md");
            string pdfPath = Path.Combine("Docs", "Blueprints", "Service-Atlanta-Revitalize-MasterPlan.pdf");
            string markdown = File.ReadAllText(mdPath);
            BlueprintPdfComposer.GenerateBlueprintPdf(markdown, pdfPath);
            Console.WriteLine($"PDF generated at: {pdfPath}");
        }
    }
}
