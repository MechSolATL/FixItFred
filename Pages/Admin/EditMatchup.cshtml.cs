using Data;
using Data.Models;

namespace Pages.Admin
{
    public class EditMatchupModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public EditMatchupModel(ApplicationDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public HeatPumpMatchup? Matchup { get; set; }  // ? Nullable to suppress CS8618

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Matchup = await _db.HeatPumpMatchups.FindAsync(id);

            return Matchup == null ? NotFound() : Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid || Matchup == null) // ? CS8601 guard
            {
                return Page();
            }

            HeatPumpMatchup? existing = await _db.HeatPumpMatchups.FindAsync(Matchup.Id);
            if (existing == null)
            {
                return NotFound();
            }

            _db.Entry(existing).CurrentValues.SetValues(Matchup);
            _ = _db.SaveChanges();

            TempData["Success"] = "Match-up updated successfully.";
            return RedirectToPage("/Admin/Matchups");
        }
    }
}
