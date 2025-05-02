using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;

namespace MVP_Core.Pages
{
    public class BlockedModel : PageModel
    {
        private readonly ILogger<BlockedModel> _logger;

        public string BlockReason { get; private set; } = "Suspicious activity detected.";
        public DateTime? BanLiftTime { get; private set; }

        public BlockedModel(ILogger<BlockedModel> logger)
        {
            _logger = logger;
        }

        public IActionResult OnGet()
        {
            _logger.LogWarning("🚫 Blocked user accessed /Blocked page. IP: {IP}", HttpContext.Connection.RemoteIpAddress);

            HttpContext.Response.StatusCode = 403; // Forbidden

            // Optionally retrieve additional block info from session or database later
            // For now, hardcode reason
            BlockReason = "Multiple security violations detected.";
            BanLiftTime = null; // Future: could populate based on database if needed

            return Page();
        }
    }
}
