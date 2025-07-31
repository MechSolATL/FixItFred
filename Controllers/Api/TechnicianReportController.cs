using Microsoft.AspNetCore.Mvc;
using MVP_Core.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using Data.Models.UI;
using Services.Reports;
using Services;

namespace Controllers.Api
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

        // POST: /api/technician/export/single
        [HttpPost("export/single")]
        public async Task<IActionResult> ExportSinglePdf([FromBody] TechnicianReportRequestViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _reportService.GenerateSingleReport(model.Id, model.FromDate, model.ToDate);
            return File(result.Content, "application/pdf", result.FileName);
        }

        // POST: /api/technician/export/comparison
        [HttpPost("export/comparison")]
        public async Task<IActionResult> ExportComparisonPdf([FromBody] TechnicianComparisonReportRequestViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _reportService.GenerateComparisonReport(model.TechIds.ToArray(), model.FromDate, model.ToDate);
            return File(result.Content, "application/pdf", result.FileName);
        }
    }
}

