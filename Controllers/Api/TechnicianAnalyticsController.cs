using Microsoft.AspNetCore.Mvc;
using MVP_Core.Services;
using MVP_Core.Data.Models;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace MVP_Core.Controllers.Api
{
    [ApiController]
    [Route("api/technician")]
    public class TechnicianAnalyticsController : ControllerBase
    {
        private readonly ITechnicianProfileService _profileService;

        public TechnicianAnalyticsController(ITechnicianProfileService profileService)
        {
            _profileService = profileService;
        }

        // GET /api/technician/{id}/analytics?from=2025-01-01&to=2025-07-24
        [HttpGet("{id:int}/analytics")]
        public async Task<ActionResult<TechnicianAnalyticsDto>> GetAnalytics(
            int id,
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to)
        {
            var range = new DateRange
            {
                Start = from ?? DateTime.UtcNow.AddDays(-30),
                End   = to   ?? DateTime.UtcNow
            };

            var analytics = await _profileService.GetAnalyticsAsync(id, range);

            if (analytics == null)
                return NotFound();

            return Ok(analytics); // includes forecasts, IsAtRisk, RiskFlags
        }

        // GET /api/technician/{id}/heatmap?start=yyyy-MM-dd&end=yyyy-MM-dd
        [HttpGet("{id:int}/heatmap")]
        public async Task<ActionResult<List<TechnicianHeatmapCell>>> GetHeatmap(
            int id,
            [FromQuery] DateTime? start,
            [FromQuery] DateTime? end)
        {
            if (_profileService == null)
                return StatusCode(500, "Profile service not available.");
            var range = new DateRange
            {
                Start = start ?? DateTime.UtcNow.AddDays(-30),
                End = end ?? DateTime.UtcNow
            };
            var service = _profileService as TechnicianProfileService;
            if (service == null)
                return StatusCode(500, "TechnicianProfileService not available.");
            var data = await service.GetHeatmapDataAsync(id, range);
            if (data == null)
                return NotFound();
            return Ok(data);
        }
    }
}
