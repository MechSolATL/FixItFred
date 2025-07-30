// Sprint92_Fix_GroupB — Razor layout set, ViewData["Title"] applied, dynamic SEO injected using ISEOService for route Patch/PatchDashboard.
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Services; // Ensure ISEOService is accessible
using MVP_Core.Data.Models.Seo;
using System.Threading.Tasks;

namespace MVP_Core.Pages.Patch
{
    public class PatchDashboardModel : PageModel
    {
        private readonly ISEOService _seoService;

        public PatchDashboardModel(ISEOService seoService)
        {
            _seoService = seoService;
        }

        public SEOModel SEO { get; set; }

        public async Task OnGetAsync()
        {
            ViewData["Title"] = "Patch Dashboard";
            Layout = "/Pages/Shared/_Layout.cshtml";

            // ?? Load SEO for Patch Dashboard
            SEO = await _seoService.GetByPageAsync("Patch/PatchDashboard");

            if (SEO != null)
            {
                ViewData["MetaDescription"] = SEO.MetaDescription;
                ViewData["MetaKeywords"] = SEO.MetaKeywords;
                ViewData["MetaRobots"] = SEO.Robots;
            }
        }
    }
}