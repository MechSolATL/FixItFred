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
