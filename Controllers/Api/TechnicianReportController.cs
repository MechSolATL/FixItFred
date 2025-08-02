using Microsoft.AspNetCore.Mvc;
using Services.Reports;
using Services;
using Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace MVP_Core.Controllers.Api
{
    [Route("api/technician")]
    [ApiController]
    public class TechnicianReportController : ControllerBase
    {
        private readonly TechnicianProfileService _profileService;
        private readonly TechnicianReportService _reportService;

        public TechnicianReportController(
            TechnicianProfileService profileService,
            TechnicianReportService reportService)
        {
            _profileService = profileService;
            _reportService = reportService;
        }

        // POST: /api/technician/{id}/export-pdf
        [HttpPost("{id}/export-pdf")]
        public async Task<IActionResult> ExportSinglePdf(
            int id,
            [FromForm] string? notes,
            [FromForm] string? chartBase64)
        {
            var tech = await _profileService.GetProfileAsync(id);
            if (tech == null)
                return NotFound($"Technician with ID {id} not found.");

            byte[] chartImg = string.IsNullOrWhiteSpace(chartBase64)
                ? Array.Empty<byte>()
                : Convert.FromBase64String(chartBase64);

            var pdf = await _reportService.GenerateSingleReport(tech, chartImg, notes);
            return File(pdf, "application/pdf", $"technician_{id}_report.pdf");
        }

        // POST: /api/technician/export-comparison-pdf
        [HttpPost("export-comparison-pdf")]
        public async Task<IActionResult> ExportComparisonPdf(
            [FromForm] string techIds,
            [FromForm] string? notes,
            [FromForm] List<string>? chartBase64s)
        {
            if (string.IsNullOrWhiteSpace(techIds))
                return BadRequest("No technician IDs provided.");

            var ids = techIds.Split(',')
                             .Select(id => int.TryParse(id, out var parsed) ? parsed : (int?)null)
                             .Where(id => id.HasValue)
                             .Select(id => id!.Value)
                             .ToList();

            if (ids.Count == 0)
                return BadRequest("Invalid technician IDs.");

            var techs = new List<TechnicianProfileDto>();
            foreach (var id in ids)
            {
                var tech = await _profileService.GetProfileAsync(id);
                if (tech != null)
                    techs.Add(tech);
            }

            var chartImgs = chartBase64s?.Select(b64 =>
            {
                try
                {
                    return Convert.FromBase64String(b64);
                }
                catch
                {
                    return Array.Empty<byte>();
                }
            }).ToList() ?? new List<byte[]>();

            var pdf = await _reportService.GenerateComparisonReport(techs, chartImgs, notes);
            return File(pdf, "application/pdf", $"technician_comparison_report.pdf");
        }
    }
}

