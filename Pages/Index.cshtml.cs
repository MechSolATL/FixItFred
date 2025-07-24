using MVP_Core.Services;
using MVP_Core.Data.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace MVP_Core.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ISeoService _seoService;
        private readonly SessionTracker _session;
        private readonly IDeviceResolver _deviceResolver;

        public IndexModel(ILogger<IndexModel> logger, ISeoService seoService, SessionTracker session, IDeviceResolver deviceResolver)
        {
            _logger = logger;
            _seoService = seoService;
            _session = session;
            _deviceResolver = deviceResolver;
        }

        public async Task OnGetAsync()
        {
            SeoMeta? seoMeta = await _seoService.GetSeoByPageNameAsync("Home");
            ViewData["Title"] = seoMeta?.Title ?? "Service Atlanta – Home";
            ViewData["MetaDescription"] = seoMeta?.MetaDescription ?? "Your trusted pros for Plumbing, HVAC, and Water Filtration.";
            ViewData["Keywords"] = seoMeta?.Keywords ?? "plumbing, heating, air conditioning, tankless water heater, HVAC, Duluth";
            ViewData["Robots"] = seoMeta?.Robots ?? "index, follow";
            ViewData["DeviceType"] = _deviceResolver.GetDeviceType(HttpContext);
        }
    }
}
