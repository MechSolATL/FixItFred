using System;
using System.IO;
using Diagnostics.FixItFredDiagnostics;
using Services.Admin;

namespace Tools.BlueprintGen
{
    class Program
    {
        static void Main(string[] args)
        {
            StartupLogger.Log("BlueprintGen", args);

            string mdPath = Path.Combine("..", "..", "Docs", "Blueprints", "Service-Atlanta-Revitalize-MasterPlan.md");
            string pdfPath = Path.Combine("..", "..", "Docs", "Blueprints", "Service-Atlanta-Revitalize-MasterPlan.pdf");
            string markdown = File.ReadAllText(mdPath);
            BlueprintPdfComposer.GenerateBlueprintPdf(markdown, pdfPath);
            Console.WriteLine($"PDF generated at: {pdfPath}");
        }
    }
}
