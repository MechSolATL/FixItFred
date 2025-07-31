// Sprint 91.7
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Services;

namespace Controllers.Api
{
    // Sprint 91.7
    [ApiController]
    [Authorize(Roles = "Admin,Dispatcher")] // Sprint 91.7
    [Route("api/[controller]/[action]")] // Sprint 91.7
    public class TechTrackingController : ControllerBase
    {
        // Sprint 91.7
        private readonly TechnicianTrackingService _trackingService;
        public TechTrackingController(TechnicianTrackingService trackingService)
        {
            _trackingService = trackingService;
        }

        // Sprint 91.7
        [HttpGet]
        public ActionResult<List<TechnicianTrackingDto>> Live()
        {
            var locations = _trackingService.GetAllTechnicianLocations();
            return Ok(locations);
        }
    }
}
