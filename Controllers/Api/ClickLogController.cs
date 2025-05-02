using Microsoft.AspNetCore.Mvc;
using MVP_Core.Data;
using MVP_Core.Data.Models;

namespace MVP_Core.Controllers.Api
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
        public async Task<IActionResult> LogClickAsync([FromBody] ClickLogInput input)
        {
            var pageVisitLog = new PageVisitLog
            {
                PageUrl = input.PageName,
                Referrer = string.Empty, // Not available from API call
                UserAgent = Request.Headers["User-Agent"].ToString(),
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                IsRealUser = true, // Assuming manual clicks = real user
                ResponseStatusCode = 200,
                VisitTimestamp = DateTime.UtcNow
            };

            _dbContext.PageVisitLogs.Add(pageVisitLog); // ✅ Correct DbSet: PageVisitLogs (NOT PageVisits)
            await _dbContext.SaveChangesAsync();

            return Ok();
        }
    }
}
