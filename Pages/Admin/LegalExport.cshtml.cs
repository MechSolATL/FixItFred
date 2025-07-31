using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Services.Admin;

namespace Pages.Admin
{
    public class LegalExportModel : PageModel
    {
        private readonly LegalExportService _legalExportService;
        public LegalExportModel(LegalExportService legalExportService)
        {
            _legalExportService = legalExportService;
        }

        [BindProperty]
        public string ExportType { get; set; } = "ServiceRequest";
        [BindProperty]
        public string EntityId { get; set; } = string.Empty;
        public string? DownloadUrl { get; set; }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (int.TryParse(EntityId, out int id))
            {
                var zipPath = await _legalExportService.ExportLegalPacketAsync(id, ExportType);
                DownloadUrl = "/LegalExports/" + Path.GetFileName(zipPath);
                // TODO: Move ZIP to wwwroot/LegalExports for download
            }
            return Page();
        }
    }
}
