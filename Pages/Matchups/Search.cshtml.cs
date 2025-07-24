// ===============================
// File: Pages/Matchups/Search.cshtml.cs
// ===============================

using MVP_Core.Services;

namespace MVP_Core.Pages.Matchups
{
    public class SearchModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly ISeoService _seoService;

        public SearchModel(ApplicationDbContext db, ISeoService seoService)
        {
            _db = db;
            _seoService = seoService;
        }

        [BindProperty(SupportsGet = true)]
        public string Query { get; set; } = string.Empty;

        public List<HeatPumpMatchup> Results { get; set; } = [];

        public async Task OnGetAsync()
        {
            ViewData["Title"] = "Search Heat Pump Matchups";
            ViewData["Layout"] = "/Pages/Shared/_Layout.cshtml";

            var seo = await _seoService.GetSeoByPageNameAsync("MatchupsSearch");
            if (seo != null)
            {
                ViewData["MetaDescription"] = seo.MetaDescription;
                ViewData["Keywords"] = seo.Keywords;
                ViewData["Robots"] = seo.Robots ?? "index, follow";
            }

            if (!string.IsNullOrWhiteSpace(Query))
            {
                string lower = Query.ToLower();
                Results = await _db.HeatPumpMatchups
                    .Where(m =>
                        m.Brand.ToLower().Contains(lower) ||
                        m.SystemType.ToLower().Contains(lower) ||
                        m.ACoilModel.ToLower().Contains(lower) ||
                        m.OutdoorUnitModel.ToLower().Contains(lower) ||
                        m.FurnaceModel.ToLower().Contains(lower) ||
                        m.SEERRating.ToLower().Contains(lower) ||
                        m.AHRICode.ToLower().Contains(lower) ||
                        m.Notes.ToLower().Contains(lower))
                    .OrderByDescending(m => m.CreatedAt)
                    .ToListAsync();
            }
        }
    }
}
