using Data;
using Data.Models;

namespace Controllers
{
    [Route("robots.txt")]
    public class RobotsController : Controller
    {
        private readonly ApplicationDbContext _db;

        public RobotsController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            RobotsContent? robots = await _db.RobotsContents.FirstOrDefaultAsync();
            string content = robots?.Content ?? "User-agent: *\nDisallow:";
            return Content(content, "text/plain");
        }
    }
}
