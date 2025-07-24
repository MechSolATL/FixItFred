using Microsoft.AspNetCore.Mvc;
using MVP_Core.Services;
using System.Threading.Tasks;

namespace MVP_Core.Controllers.Api
{
    [ApiController]
    public class TechnicianApiController : ControllerBase
    {
        [HttpGet("/api/technicians/active")]
        public async Task<IActionResult> GetActiveTechnicians([FromServices] ITechnicianService techService)
        {
            var techs = await techService.GetAllAsync();
            var activeTechs = techs.Where(t => t.IsActive).ToList();
            return Ok(activeTechs);
        }
    }
}
