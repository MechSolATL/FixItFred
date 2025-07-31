using System;
using System.IO;

namespace Services.Admin
{
    // Sprint 90.0: Utility to generate Service-Atlanta Revitalize MasterPlan PDF from Markdown
    public static class BlueprintGenerator
    {
        public static void GenerateMasterPlanPdf()
        {
            string mdPath = Path.Combine("Docs", "Blueprints", "Service-Atlanta-Revitalize-MasterPlan.md");
            string pdfPath = Path.Combine("Docs", "Blueprints", "Service-Atlanta-Revitalize-MasterPlan.pdf");
            string markdown = File.ReadAllText(mdPath);
            BlueprintPdfComposer.GenerateBlueprintPdf(markdown, pdfPath);
        }
    }
}
