using System.Threading.Tasks;
using Data.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;

namespace Pages
{
    // This file has been patched in Sprint 91.9-B
    public class AccessDeniedModel : PageModel
    {
        private readonly ISeoService _seoService;

        public AccessDeniedModel(ISeoService seoService)
        {
            _seoService = seoService;
        }

        public SeoMeta Seo { get; set; } = new SeoMeta();
        public string TierStatus { get; set; } = "Basic";
        public string ViewTitle { get; set; } = "Access Denied";
        public string? ReturnUrl { get; set; }

        public async Task OnGetAsync()
        {
            var seo = await _seoService.GetSeoByPageNameAsync("AccessDenied");
            if (seo != null)
            {
                ViewData["Title"] = seo.Title;
                ViewData["MetaDescription"] = seo.MetaDescription;
                ViewData["Keywords"] = seo.Keywords;
                ViewData["Robots"] = seo.Robots;
            }
        }
    }
}
