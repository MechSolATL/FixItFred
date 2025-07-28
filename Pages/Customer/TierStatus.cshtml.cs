using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Services;
using System.Collections.Generic;
using MVP_Core.Data.Models;

namespace MVP_Core.Pages.Customer
{
    // Sprint 84.6: Customer Tier Viewer scaffold
    public class TierStatusModel : PageModel
    {
        public int TotalPoints { get; set; }
        public RewardTier? CurrentTier { get; set; }
        public RewardTier? NextTier { get; set; }
        public int ProgressPercent { get; set; }
        public bool HasBronze { get; set; }
        public List<RewardTier> Tiers { get; set; } = new();
        public void OnGet()
        {
            var email = User.Identity?.Name ?? string.Empty;
            var portalService = HttpContext.RequestServices.GetService(typeof(CustomerPortalService)) as CustomerPortalService;
            var loyalty = portalService?.GetLoyaltyTransactions(email) ?? new List<LoyaltyPointTransaction>();
            Tiers = portalService?.GetRewardTiers() ?? new List<RewardTier>();
            TotalPoints = loyalty.Sum(l => l.Points);
            CurrentTier = Tiers.LastOrDefault(t => TotalPoints >= t.PointsRequired);
            NextTier = Tiers.FirstOrDefault(t => t.PointsRequired > TotalPoints);
            ProgressPercent = NextTier != null && NextTier.PointsRequired > 0 ? (int)(100.0 * TotalPoints / NextTier.PointsRequired) : 100;
            HasBronze = CurrentTier != null && CurrentTier.PointsRequired >= (Tiers.FirstOrDefault(t => t.Name == "Bronze")?.PointsRequired ?? 0);
        }
    }
}
