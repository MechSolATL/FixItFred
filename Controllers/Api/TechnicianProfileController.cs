using Microsoft.AspNetCore.Mvc;
using MVP_Core.Services;
using MVP_Core.Data.Models;

namespace MVP_Core.Controllers.Api
{
    [Route("Admin/TechnicianProfile")]
    public class TechnicianProfileController : Controller
    {
        private readonly TechnicianProfileService _profileService;
        public TechnicianProfileController(TechnicianProfileService profileService)
        {
            _profileService = profileService;
        }

        [HttpGet("GetProfile")]
        public async Task<IActionResult> GetProfile(int techId)
        {
            var profile = await _profileService.GetProfileAsync(techId);
            if (profile == null) return NotFound();
            return PartialView("~/Pages/Admin/_TechnicianProfileModal.cshtml", profile);
        }

        [HttpGet("ExportCSV")]
        public async Task<IActionResult> ExportCSV(int techId)
        {
            var bytes = await _profileService.ExportCsvAsync(techId);
            return File(bytes, "text/csv", $"technician_{techId}_profile.csv");
        }

        [HttpGet("ExportPDF")]
        public async Task<IActionResult> ExportPDF(int techId)
        {
            var bytes = await _profileService.ExportPdfAsync(techId);
            return File(bytes, "application/pdf", $"technician_{techId}_profile.pdf");
        }

        [HttpGet("/api/technician/{id}/analytics")]
        public async Task<IActionResult> GetAnalytics(int id, DateTime start, DateTime end)
        {
            var range = new MVP_Core.Data.Models.DateRange { Start = start, End = end };
            var analytics = await _profileService.GetAnalyticsAsync(id, range);
            return Ok(analytics);
        }

        [HttpPost("/api/technician/export-csv")]
        public async Task<IActionResult> ExportCsv([FromForm] string techIds)
        {
            var ids = techIds.Split(',').Select(int.Parse).ToList();
            if (ids.Count == 1)
            {
                var bytes = await _profileService.ExportCsvAsync(ids[0]);
                return File(bytes, "text/csv", $"technician_{ids[0]}_analytics.csv");
            }
            else
            {
                var bytes = await _profileService.ExportCsvAsync(ids);
                return File(bytes, "text/csv", $"technician_comparison_analytics.csv");
            }
        }
    }
}
