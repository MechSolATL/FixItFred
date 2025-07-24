using Microsoft.AspNetCore.Mvc.Rendering;

namespace MVP_Core.Pages.Admin
{
    public class MatchupsModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public MatchupsModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public List<HeatPumpMatchup> FilteredMatchups { get; set; } = [];
        public List<SelectListItem> Brands { get; set; } = [];
        public List<SelectListItem> SystemTypes { get; set; } = [];

        [BindProperty(SupportsGet = true)]
        public string? SelectedBrand { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SelectedSystemType { get; set; }

        public async Task OnGetAsync()
        {
            IQueryable<HeatPumpMatchup> query = _db.HeatPumpMatchups.AsQueryable();

            if (!string.IsNullOrEmpty(SelectedBrand))
            {
                query = query.Where(m => m.Brand == SelectedBrand);
            }

            if (!string.IsNullOrEmpty(SelectedSystemType))
            {
                query = query.Where(m => m.SystemType == SelectedSystemType);
            }

            FilteredMatchups = await query.OrderBy(m => m.Brand).ThenBy(m => m.SystemType).ToListAsync();

            Brands = await _db.HeatPumpMatchups
                .Select(m => m.Brand)
                .Distinct()
                .OrderBy(b => b)
                .Select(b => new SelectListItem { Value = b, Text = b })
                .ToListAsync();

            SystemTypes = await _db.HeatPumpMatchups
                .Select(m => m.SystemType)
                .Distinct()
                .OrderBy(t => t)
                .Select(t => new SelectListItem { Value = t, Text = t })
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            HeatPumpMatchup? matchup = await _db.HeatPumpMatchups.FindAsync(id);
            if (matchup == null)
            {
                return NotFound();
            }

            _ = _db.HeatPumpMatchups.Remove(matchup);
            _ = _db.SaveChanges();

            TempData["Message"] = "Entry deleted.";
            return RedirectToPage();
        }
    }
}
