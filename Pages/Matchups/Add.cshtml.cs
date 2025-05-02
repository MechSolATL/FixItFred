// =========================
// File: Pages/Matchups/Add.cshtml.cs
// =========================
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MVP_Core.Data;
using MVP_Core.Data.Models;
using System.Threading.Tasks;

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

            var seo = await _db.SEOs.FirstOrDefaultAsync(x => x.PageName == "MatchupsAdd");
            if (seo != null)
            {
                ViewData["MetaDescription"] = seo.MetaDescription;
                ViewData["Keywords"] = seo.Keywords;
                ViewData["Robots"] = "noindex, nofollow"; // Adjust if needed
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(Match.ACoilModel) || string.IsNullOrWhiteSpace(Match.OutdoorUnitModel))
            {
                Message = "⚠️ Please fill in at least A-Coil and Outdoor Unit model.";
                IsSuccess = false;
                return Page();
            }

            Match.CreatedAt = DateTime.UtcNow;
            _db.HeatPumpMatchups.Add(Match);
            await _db.SaveChangesAsync();

            Message = "✅ Heat pump match-up successfully saved.";
            IsSuccess = true;
            ModelState.Clear();
            Match = new HeatPumpMatchup();
            return Page();
        }
    }
}
