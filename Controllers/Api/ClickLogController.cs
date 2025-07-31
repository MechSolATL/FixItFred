using Data;
using Data.Models;

namespace Controllers.Api
{
    [ApiController]
    [Route("api/click-log")]
    public class ClickLogController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public ClickLogController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public class ClickLogInput
        {
            public string ClickText { get; set; } = string.Empty;
            public string PageName { get; set; } = "/";
        }

        [HttpPost]
        public IActionResult LogClick([FromBody] ClickLogInput input)
        {
            PageVisitLog pageVisitLog = new()
            {
                PageUrl = input.PageName,
                Referrer = string.Empty, // Not available from API call
                UserAgent = Request.Headers["User-Agent"].ToString(),
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                IsRealUser = true, // Assuming manual clicks = real user
                ResponseStatusCode = 200,
                VisitTimestamp = DateTime.UtcNow
            };
            _ = _dbContext.PageVisitLogs.Add(pageVisitLog); // ? Correct DbSet: PageVisitLogs (NOT PageVisits)
            _ = _dbContext.SaveChanges();

            return Ok();
        }
    }
}
