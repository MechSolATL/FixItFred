using Data;
using Data.Models;
using Data.Models.Seo;
using Microsoft.AspNetCore.Authorization;
using Services;

namespace Pages.Admin
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

        public SeoMetadata Seo { get; set; } = new SeoMetadata();

        public string Title => Seo.Title;
        public string MetaDescription => Seo.MetaDescription;
        public string Keywords => Seo.Keywords;
        public string Robots => Seo.Robots;

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
