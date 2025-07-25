using MVP_Core.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using MVP_Core.Data;
using MVP_Core.Data.Models;
using System.Linq;
using System.Threading.Tasks;

namespace MVP_Core.Pages.Services
{
    public class WaterFiltrationModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<WaterFiltrationModel> _logger;
        private readonly ISeoService _seoService;
        private readonly IDeviceResolver _deviceResolver;

        public WaterFiltrationModel(ApplicationDbContext context, ILogger<WaterFiltrationModel> logger, ISeoService seoService, IDeviceResolver deviceResolver)
        {
            _context = context;
            _logger = logger;
            _seoService = seoService;
            _deviceResolver = deviceResolver;
        }

        // SignalR ETA tracking keys (stubbed for simulation)
        public int TechnicianId { get; set; } = 1; // Simulated technician
        public int RequestId { get; set; } = 1001; // Simulated request

        // FixItFred Patch Log — Sprint 29A Recovery
        // [2025-07-25T00:00:00Z] — Added UserAnswer property for Razor binding and SignalR ETA integration.
        public string UserAnswer { get; set; } = string.Empty;

        public async Task OnGetAsync()
        {
            var seo = await _seoService.GetSeoByPageNameAsync("Services/WaterFiltration");
            ViewData["Title"] = seo?.Title ?? "WaterFiltration Page";
            ViewData["MetaDescription"] = seo?.MetaDescription;
            ViewData["Keywords"] = seo?.Keywords;
            ViewData["Robots"] = seo?.Robots;
            ViewData["DeviceType"] = _deviceResolver.GetDeviceType(HttpContext);
        }
    }
}
