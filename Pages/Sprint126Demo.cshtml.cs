using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Pages
{
    /// <summary>
    /// [Sprint126_OneScan_91-99] Demo page model for Sprint 126 OneScan implementation
    /// Demonstrates all implemented features working together
    /// </summary>
    public class Sprint126DemoModel : PageModel
    {
        private readonly ILogger<Sprint126DemoModel> _logger;

        public Sprint126DemoModel(ILogger<Sprint126DemoModel> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// [Sprint126_OneScan_91-99] Loads the demo page
        /// </summary>
        public void OnGet()
        {
            _logger.LogInformation("[Sprint126_OneScan_91-99] Loading Sprint 126 demo page");
            
            ViewData["Title"] = "Sprint 126 OneScan Demo";
            ViewData["Description"] = "Complete demonstration of MVP-Core Sprint 126 OneScan implementation";
        }
    }
}