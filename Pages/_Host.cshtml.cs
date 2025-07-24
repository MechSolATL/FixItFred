namespace MVP_Core.Pages
{
    public class _HostModel : PageModel
    {
        private readonly ISeoService _seoService;

        public _HostModel(ISeoService seoService)
        {
            _seoService = seoService;
        }

        public async Task OnGetAsync()
        {
            SeoMeta? SeoMeta = await _seoService.GetSeoByPageNameAsync("_Host");

            ViewData["Title"] = SeoMeta?.Title ?? "MechSol ATL – Developer Hub";
            ViewData["MetaDescription"] = SeoMeta?.MetaDescription ?? "Developer hub for managing and testing service workflows.";
            ViewData["Keywords"] = SeoMeta?.Keywords ?? "Service Atlanta, Dev Tools, Workflow Testing";
            ViewData["Robots"] = SeoMeta?.Robots ?? "noindex, nofollow";
        }
    }
}
