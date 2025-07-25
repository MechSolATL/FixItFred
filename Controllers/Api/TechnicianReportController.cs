using Microsoft.AspNetCore.Mvc;
using MVP_Core.Services.Reports;
using MVP_Core.Services;
using MVP_Core.Data.Models;
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
        public TechnicianReportController(TechnicianProfileService profileService, TechnicianReportService reportService)
        {
            _profileService = profileService;
            _reportService = reportService;
        }

        [HttpPost("{id}/export-pdf")]
        public async Task<IActionResult> ExportSinglePdf(int id, [FromForm] string? notes, [FromForm] string? chartBase64)
        {
            var tech = await _profileService.GetProfileAsync(id);
            if (tech == null) return NotFound();
            byte[] chartImg = string.IsNullOrEmpty(chartBase64) ? Array.Empty<byte>() : Convert.FromBase64String(chartBase64);
            var pdf = _reportService.GenerateSingleReport(tech, chartImg, notes);
            return File(pdf, "application/pdf", $"technician_{id}_report.pdf");
        }

        [HttpPost("export-comparison-pdf")]
        public async Task<IActionResult> ExportComparisonPdf([FromForm] string techIds, [FromForm] string? notes, [FromForm] List<string>? chartBase64s)
        {
            var ids = techIds.Split(',').Select(int.Parse).ToList();
            var techs = new List<TechnicianProfileDto>();
            foreach (var id in ids)
            {
                var t = await _profileService.GetProfileAsync(id);
                if (t != null) techs.Add(t);
            }
            var chartImgs = chartBase64s?.Select(b64 => Convert.FromBase64String(b64)).ToList() ?? new List<byte[]>();
            var pdf = _reportService.GenerateComparisonReport(techs, chartImgs, notes);
            return File(pdf, "application/pdf", $"technician_comparison_report.pdf");
        }
    }
}
