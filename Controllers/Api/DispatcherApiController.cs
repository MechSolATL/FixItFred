// Sprint 48.2 Patch Log: SuggestBestTech API endpoint
// [Sprint1002_FixItFred] Fixed namespace reference to use correct Models.Admin
using Microsoft.AspNetCore.Mvc;
using Models.Admin;
using Microsoft.AspNetCore.SignalR;
using Hubs;
using Services.Admin;

namespace Controllers.Api
{
    [Route("api/dispatcher")]
    [ApiController]
    public class DispatcherApiController : ControllerBase
    {
        private readonly DispatcherService _dispatcherService;
        private readonly IHubContext<RequestHub> _hubContext;
        public DispatcherApiController(DispatcherService dispatcherService, IHubContext<RequestHub> hubContext)
        {
            _dispatcherService = dispatcherService;
            _hubContext = hubContext;
        }
        [HttpGet("suggest-best-tech")]
        public IActionResult SuggestBestTech([FromQuery] int requestId)
        {
            var result = _dispatcherService.SuggestBestTechnician(requestId);
            if (result.Technician == null)
                return NotFound(new { technicianName = "None", score = 0, reason = result.Reason });
            // Optionally broadcast SLA warning if fallback
            if (!string.IsNullOrEmpty(result.Reason))
                _dispatcherService.BroadcastSLAWarningAsync(requestId, $"SLA Warning: {result.Reason}", _hubContext);
            return Ok(new {
                technicianName = result.Technician.Name,
                score = result.Score,
                reason = result.Reason
            });
        }
    }
}
