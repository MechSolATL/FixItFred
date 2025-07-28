using Microsoft.AspNetCore.Mvc;
using MVP_Core.Data;
using MVP_Core.Data.Models;
using System.Linq;
using System.Threading.Tasks;

namespace MVP_Core.Controllers
{
    [ApiController]
    [Route("api/loyalty")]
    public class LoyaltyController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public LoyaltyController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet("{techId}")]
        public async Task<IActionResult> GetLoyaltyStatus(int techId)
        {
            var milestones = _context.TechMilestones.ToList();
            var unlocked = _context.MilestoneAuditLogs.Where(x => x.TechnicianId == techId && x.Action == "Unlocked").ToList();
            return Ok(new {
                milestones,
                unlocked
            });
        }
    }
}
