using Microsoft.AspNetCore.Authorization;
using MVP_Core.Services;

namespace MVP_Core.Pages.Admin
{
    [Authorize(Policy = "AdminPolicy")]
    [ValidateAntiForgeryToken]
    public class ManageSEOModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ISeoService _seoService;
        private readonly IDeviceResolver _deviceResolver;

        public List<SeoMeta> SEOs { get; set; } = [];

        public ManageSEOModel(ApplicationDbContext dbContext, ISeoService seoService, IDeviceResolver deviceResolver)
        {
            _dbContext = dbContext;
            _seoService = seoService;
            _deviceResolver = deviceResolver;
        }

        public async Task OnGetAsync()
        {
            var seo = await _seoService.GetSeoByPageNameAsync("Admin/ManageSEO");
            ViewData["Title"] = seo?.Title ?? "Manage SEO Metadata";
            ViewData["MetaDescription"] = seo?.MetaDescription ?? "Admin panel for managing SEO metadata.";
            ViewData["Keywords"] = seo?.Keywords;
            ViewData["Robots"] = seo?.Robots ?? "noindex, nofollow";
            ViewData["DeviceType"] = _deviceResolver.GetDeviceType(HttpContext);
            SEOs = await _dbContext.SEOs
                .OrderBy(static s => s.PageName)
                .ToListAsync();
        }
    }
}
