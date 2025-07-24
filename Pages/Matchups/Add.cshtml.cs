// =========================
// File: Pages/Matchups/Add.cshtml.cs
// =========================
using Microsoft.AspNetCore.Authorization;

namespace MVP_Core.Pages.Matchups
{
    [Authorize(Roles = "Admin")]
    public class AddModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public AddModel(ApplicationDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public HeatPumpMatchup Match { get; set; } = new();

        public string? Message { get; set; }
        public bool IsSuccess { get; set; } = false;

        public async Task OnGetAsync()
        {
            ViewData["Title"] = "Add Heat Pump Matchup";
            ViewData["Layout"] = "/Pages/Shared/_Layout.cshtml";

            SeoMeta? SeoMeta = await _db.SEOs.FirstOrDefaultAsync(static x => x.PageName == "MatchupsAdd");
            if (SeoMeta != null)
            {
                ViewData["MetaDescription"] = SeoMeta.MetaDescription;
                ViewData["Keywords"] = SeoMeta.Keywords;
                ViewData["Robots"] = "noindex, nofollow"; // Adjust if needed
            }
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(Match.ACoilModel) || string.IsNullOrWhiteSpace(Match.OutdoorUnitModel))
            {
                Message = "?? Please fill in at least A-Coil and Outdoor Unit model.";
                IsSuccess = false;
                return Page();
            }

            Match.CreatedAt = DateTime.UtcNow;
            _ = _db.HeatPumpMatchups.Add(Match);
            _ = _db.SaveChanges();

            Message = "? Heat pump match-up successfully saved.";
            IsSuccess = true;
            ModelState.Clear();
            Match = new HeatPumpMatchup();
            return Page();
        }
    }
}
