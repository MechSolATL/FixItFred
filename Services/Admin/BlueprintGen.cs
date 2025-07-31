using System;
using System.IO;

// Standalone console app for generating the PDF blueprint
namespace Services.Admin
{
    class BlueprintGen
    {
        static void Main(string[] args)
        {
            string mdPath = System.IO.Path.Combine("Docs", "Blueprints", "Service-Atlanta-Revitalize-MasterPlan.md");
            string pdfPath = System.IO.Path.Combine("Docs", "Blueprints", "Service-Atlanta-Revitalize-MasterPlan.pdf");
            string markdown = System.IO.File.ReadAllText(mdPath);
            BlueprintPdfComposer.GenerateBlueprintPdf(markdown, pdfPath);
            System.Console.WriteLine($"PDF generated at: {pdfPath}");
        }
    }
}
