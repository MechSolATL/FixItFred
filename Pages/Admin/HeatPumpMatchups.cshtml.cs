using Data;
using Data.Models;

namespace Pages.Admin
{
    public class HeatPumpMatchupsModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public HeatPumpMatchupsModel(ApplicationDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public HeatPumpMatchup NewMatchup { get; set; } = new();

        public List<HeatPumpMatchup> Matchups { get; set; } = [];

        public async Task OnGetAsync()
        {
            Matchups = await _db.HeatPumpMatchups
                .OrderByDescending(static m => m.CreatedAt)
                .ToListAsync();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            NewMatchup.CreatedAt = DateTime.UtcNow;
            _ = _db.HeatPumpMatchups.Add(NewMatchup);
            _ = _db.SaveChanges();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            HeatPumpMatchup? match = await _db.HeatPumpMatchups.FindAsync(id);
            if (match != null)
            {
                _ = _db.HeatPumpMatchups.Remove(match);
                _ = _db.SaveChanges();
            }

            return RedirectToPage();
        }
    }
}
