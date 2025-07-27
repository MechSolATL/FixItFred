using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Data.Models;
using MVP_Core.Services.Admin;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace MVP_Core.Pages.Admin
{
    public class LoyaltyCenterModel : PageModel
    {
        private readonly TechnicianLoyaltyService _loyaltyService;
        private readonly ApplicationDbContext _db;
        public LoyaltyCenterModel(TechnicianLoyaltyService loyaltyService, ApplicationDbContext db)
        {
            _loyaltyService = loyaltyService;
            _db = db;
            LoyaltyLogs = new List<TechnicianLoyaltyLog>();
        }
        [BindProperty]
        public string FilterTier { get; set; } = "";
        [BindProperty]
        public string SortBy { get; set; } = "";
        public List<TechnicianLoyaltyLog> LoyaltyLogs { get; set; }
        [BindProperty]
        public int AwardTechnicianId { get; set; }
        [BindProperty]
        public int AwardPoints { get; set; }
        [BindProperty]
        public string AwardMilestone { get; set; } = "";
        [BindProperty]
        public string AwardNotes { get; set; } = "";
        public async Task OnGetAsync()
        {
            LoyaltyLogs = await _db.TechnicianLoyaltyLogs.OrderByDescending(x => x.AwardedAt).ToListAsync();
        }
        public async Task<IActionResult> OnPostFilterAsync()
        {
            if (!string.IsNullOrEmpty(FilterTier))
                LoyaltyLogs = await _loyaltyService.GetByTierAsync(FilterTier);
            else
                LoyaltyLogs = await _db.TechnicianLoyaltyLogs.OrderByDescending(x => x.AwardedAt).ToListAsync();
            return Page();
        }
        public async Task<IActionResult> OnPostSortAsync()
        {
            if (SortBy == "MostImproved")
                LoyaltyLogs = await _loyaltyService.GetMostImprovedAsync();
            else
                LoyaltyLogs = await _db.TechnicianLoyaltyLogs.OrderByDescending(x => x.AwardedAt).ToListAsync();
            return Page();
        }
        public async Task<IActionResult> OnPostAwardAsync()
        {
            await _loyaltyService.AwardPointsAsync(AwardTechnicianId, AwardPoints, AwardMilestone, AwardNotes);
            LoyaltyLogs = await _db.TechnicianLoyaltyLogs.OrderByDescending(x => x.AwardedAt).ToListAsync();
            return Page();
        }
    }
}
