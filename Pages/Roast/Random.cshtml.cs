// Sprint 83.6-RoastRoulette
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Data;
using Services;

namespace Pages.Roast
{
    // Sprint 83.6-RoastRoulette
    public class RandomModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly RoastRouletteService _rouletteService;
        public RandomModel(ApplicationDbContext db)
        {
            _db = db;
            _rouletteService = new RoastRouletteService(db);
        }

        [BindProperty]
        public RoastRouletteResult? SelectedRoast { get; set; }
        public string RoastMessage { get; set; } = string.Empty;

        public async Task OnGetAsync()
        {
            // Default to Medium tier for demo; could be user/session driven
            var result = await _rouletteService.GetWeightedRandomRoastAsync(RoastTier.Medium);
            if (result != null)
            {
                SelectedRoast = result;
                var template = await _db.RoastTemplates.FindAsync(result.RoastTemplateId);
                RoastMessage = template?.Message ?? "";
            }
        }

        public async Task<IActionResult> OnPostAsync(string action, int RoastTemplateId)
        {
            var userId = User?.Identity?.Name ?? "anonymous";
            if (action == "confirm")
            {
                await _rouletteService.LogDeliveryAsync(userId, RoastTemplateId, "RoastRoulette", "Confirmed");
                TempData["SystemMessages"] = "Roast delivered and logged.";
            }
            else
            {
                await _rouletteService.LogDeliveryAsync(userId, RoastTemplateId, "RoastRoulette", "Cancelled");
                TempData["SystemMessages"] = "Roast cancelled.";
            }
            return RedirectToPage();
        }
    }
}
