using MVP_Core.Data;
using MVP_Core.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MVP_Core.Services
{
    public class PromotionEngineService
    {
        private readonly ApplicationDbContext _db;
        public PromotionEngineService(ApplicationDbContext db)
        {
            _db = db;
        }

        // Get active promotions
        public List<PromotionEvent> GetActivePromotions(string? zone = null, string? serviceType = null)
        {
            var now = DateTime.UtcNow;
            return _db.PromotionEvents
                .Where(p => p.Active && p.StartDate <= now && p.EndDate >= now
                    && (zone == null || p.Zone == zone)
                    && (serviceType == null || p.ServiceType == serviceType))
                .ToList();
        }

        // Check if customer qualifies for a promotion
        public List<PromotionEvent> GetQualifiedPromotions(string customerEmail)
        {
            var customer = _db.Customers.FirstOrDefault(c => c.Email == customerEmail);
            if (customer == null) return new();
            var jobsCompleted = _db.ServiceRequests.Count(r => r.Email == customerEmail && r.Status == "Completed");
            var referrals = _db.ReferralEventLogs.Count(r => r.ReferrerCustomerId == customer.Id);
            var fiveStarStreak = _db.CustomerReviews.Count(r => r.CustomerId == customer.Id && r.Rating == 5);
            var now = DateTime.UtcNow;
            var promos = _db.PromotionEvents.Where(p => p.Active && p.StartDate <= now && p.EndDate >= now).ToList();
            var qualified = new List<PromotionEvent>();
            foreach (var promo in promos)
            {
                if (promo.TriggerType == "JobsCompleted" && jobsCompleted >= promo.MinJobsRequired)
                    qualified.Add(promo);
                if (promo.TriggerType == "Referrals" && referrals >= promo.ReferralBonus)
                    qualified.Add(promo);
                if (promo.TriggerType == "ReviewStreak" && fiveStarStreak >= (promo.StreakRequired ?? 0))
                    qualified.Add(promo);
            }
            return qualified;
        }

        // Award bonus to customer
        public void AwardBonus(string customerEmail, PromotionEvent promo)
        {
            var log = new CustomerBonusLog
            {
                CustomerEmail = customerEmail,
                PromotionId = promo.Id,
                RewardType = promo.RewardType,
                DateEarned = DateTime.UtcNow,
                IsClaimed = false,
                SourceTrigger = promo.TriggerType,
                LoyaltyPointsAwarded = promo.LoyaltyPointsAwarded,
                DiscountAmount = promo.DiscountAmount
            };
            _db.CustomerBonusLogs.Add(log);
            _db.SaveChanges();
        }

        // Claim bonus
        public bool ClaimBonus(int bonusLogId)
        {
            var log = _db.CustomerBonusLogs.FirstOrDefault(l => l.Id == bonusLogId);
            if (log == null || log.IsClaimed) return false;
            log.IsClaimed = true;
            log.DateClaimed = DateTime.UtcNow;
            _db.SaveChanges();
            return true;
        }
    }
}
