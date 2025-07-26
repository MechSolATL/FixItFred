using System.Threading.Tasks;
using MVP_Core.Services.Admin;
// ...existing code...
public class ExportController : Controller
{
    [HttpPost]
    [Route("api/investor/export-pack")]
    public async Task<IActionResult> ExportInvestorPack([FromForm] string summary, [FromForm] string architectureDiagramPath, [FromForm] string featureMatrix, [FromForm] string revenueModel, [FromForm] string pitchPack, [FromForm] string sprintTag)
    {
        string outputDir = Path.Combine(Directory.GetCurrentDirectory(), "InvestorExports");
        await InvestorExportPipeline.GenerateInvestorPackAsync(summary, architectureDiagramPath, featureMatrix, revenueModel, pitchPack, outputDir, sprintTag);
        string pdfPath = Path.Combine(outputDir, $"InvestorPack_{sprintTag}.pdf");
        return PhysicalFile(pdfPath, "application/pdf", $"InvestorPack_{sprintTag}.pdf");
    }
}
// ...existing code...