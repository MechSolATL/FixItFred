using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Services;
using MVP_Core.Data.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace MVP_Core.Pages.Admin
{
    /// <summary>
    /// PageModel for RewardManager admin page. Allows tier management.
    /// </summary>
    public class RewardManagerModel : PageModel
    {
        private readonly LoyaltyRewardService _loyaltyService;
        public RewardManagerModel(LoyaltyRewardService loyaltyService)
        {
            _loyaltyService = loyaltyService;
        }

        public void OnGet() { }

        public IActionResult OnPost()
        {
            var name = Request.Form["Name"].ToString();
            var pointsRequired = int.TryParse(Request.Form["PointsRequired"], out var pr) ? pr : 0;
            var bonusPoints = int.TryParse(Request.Form["BonusPoints"], out var bp) ? bp : 0;
            var description = Request.Form["Description"].ToString();
            var badgeIcon = Request.Form["BadgeIcon"].ToString();
            var isActive = Request.Form["IsActive"] == "true";
            var tier = new RewardTier
            {
                Name = name,
                PointsRequired = pointsRequired,
                BonusPoints = bonusPoints,
                Description = description,
                BadgeIcon = badgeIcon,
                IsActive = isActive
            };
            _loyaltyService.AddTier(tier);
            return RedirectToPage();
        }
    }
}
