using Services;

namespace Pages.Shared
{
    public class ThankYouModel : PageModel
    {
        private readonly ISeoService _seoService;

        public ThankYouModel(ISeoService seoService)
        {
            _seoService = seoService;
        }

        public async Task OnGetAsync()
        {
            var seo = await _seoService.GetSeoByPageNameAsync("ThankYou");
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