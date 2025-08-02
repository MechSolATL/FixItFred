using System;
using System.IO;
using Services.Admin;

namespace BlueprintGen
{
    class Program
    {
        static void Main(string[] args)
        {
            string mdPath = Path.Combine("..", "..", "Docs", "Blueprints", "Service-Atlanta-Revitalize-MasterPlan.md");
            string pdfPath = Path.Combine("..", "..", "Docs", "Blueprints", "Service-Atlanta-Revitalize-MasterPlan.pdf");
            string markdown = File.ReadAllText(mdPath);
            BlueprintPdfComposer.GenerateBlueprintPdf(markdown, pdfPath);
            Console.WriteLine($"PDF generated at: {pdfPath}");
        }
    }
}
