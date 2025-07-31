using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Services;

namespace MVP_Core.Pages
{
    // This file has been patched in Sprint 91.9-B
    public class AccessDeniedModel : PageModel
    {
        private readonly ISeoService _seoService;

        public AccessDeniedModel(ISeoService seoService)
        {
            _seoService = seoService;
        }

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
