using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Services;
using MVP_Core.Data.Models;
using System.Collections.Generic;

namespace MVP_Core.Pages.Customer
{
    public class RewardsModel : PageModel
    {
        private readonly CustomerPortalService _portalService;
        public RewardsModel(CustomerPortalService portalService)
        {
            _portalService = portalService;
        }
        public List<LoyaltyPointTransaction> Loyalty { get; set; } = new();
        public List<RewardTier> Tiers { get; set; } = new();
        public RewardTier? CurrentTier { get; set; }
        public void OnGet()
        {
            if (!User.Identity.IsAuthenticated || !User.HasClaim("IsCustomer", "true"))
            {
                return;
            }
            var email = User.Identity.Name ?? string.Empty;
            Loyalty = _portalService.GetLoyaltyTransactions(email);
            Tiers = _portalService.GetRewardTiers();
            CurrentTier = Tiers.LastOrDefault(t => Loyalty.Sum(l => l.Points) >= t.PointsRequired);
        }
    }
}
