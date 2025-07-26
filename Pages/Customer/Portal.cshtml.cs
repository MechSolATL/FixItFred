using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Services;
using MVP_Core.Data.Models;
using MVP_Core.Data;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System;

namespace MVP_Core.Pages.Customer
{
    public class PortalModel : PageModel
    {
        private readonly CustomerPortalService _portalService;
        private readonly ApplicationDbContext _db;
        private readonly PromotionEngineService _promoService;
        public MVP_Core.Data.Models.Customer? Customer { get; set; }
        public List<MVP_Core.Data.Models.ServiceRequest> Requests { get; set; } = new();
        public List<MVP_Core.Data.Models.LoyaltyPointTransaction> Loyalty { get; set; } = new();
        public MVP_Core.Data.Models.RewardTier? CurrentTier { get; set; }
        public bool HasPendingFollowUp { get; set; } = false;
        public bool ReviewBannerActive { get; set; } = false;
        public PortalModel(CustomerPortalService portalService, ApplicationDbContext db, PromotionEngineService promoService)
        {
            _portalService = portalService;
            _db = db;
            _promoService = promoService;
        }
        public void OnGet()
        {
            if (!User.Identity.IsAuthenticated || !User.HasClaim("IsCustomer", "true"))
            {
                return;
            }
            var email = User.Identity.Name ?? string.Empty;
            Requests = _portalService.GetServiceRequests(email);
            Loyalty = _portalService.GetLoyaltyTransactions(email);
            Customer = _portalService.GetCustomer(email);
            CurrentTier = _portalService.GetRewardTiers().LastOrDefault(t => Loyalty.Sum(l => l.Points) >= t.PointsRequired);
            // Sprint 52.0: Check for pending follow-up actions
            if (Customer != null)
            {
                HasPendingFollowUp = _db?.FollowUpActionLogs.Any(l => l.Status == "Pending" && l.UserId == Customer.Id) ?? false;
                ReviewBannerActive = _db?.FollowUpActionLogs.Any(l => l.Status == "Active" && l.TriggerType == "NoReviewBanner" && l.UserId == Customer.Id) ?? false;
            }
        }
        public IActionResult OnPostClaimBonus(int promoId)
        {
            var email = User.Identity?.Name ?? string.Empty;
            var promo = _db.PromotionEvents.FirstOrDefault(p => p.Id == promoId);
            if (promo != null)
            {
                _promoService.AwardBonus(email, promo);
            }
            return RedirectToPage();
        }
        public IActionResult OnPostClaimBonusLog(int bonusLogId)
        {
            _promoService.ClaimBonus(bonusLogId);
            return RedirectToPage();
        }
    }
}
