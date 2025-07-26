using MVP_Core.Data.Models;
using MVP_Core.Data;
using System;
using System.Linq;
using System.Collections.Generic;

namespace MVP_Core.Services
{
    /// <summary>
    /// Handles loyalty points, reward tiers, and promo generation for customers.
    /// </summary>
    public class LoyaltyRewardService
    {
        private readonly ApplicationDbContext _db;
        public LoyaltyRewardService(ApplicationDbContext db)
        {
            _db = db;
        }

        // Award points for a review or job completion
        public void AwardPoints(int customerId, int points, string type, string? description = null, int? relatedReviewId = null)
        {
            var txn = new LoyaltyPointTransaction
            {
                CustomerId = customerId,
                Points = points,
                Type = type,
                Description = description,
                RelatedReviewId = relatedReviewId
            };
            _db.LoyaltyPointTransactions.Add(txn);
            _db.SaveChanges();
        }

        // Get total points for a customer
        public int GetTotalPoints(int customerId)
        {
            return _db.LoyaltyPointTransactions.Where(t => t.CustomerId == customerId).Sum(t => t.Points);
        }

        // Get current tier for a customer
        public RewardTier? GetCurrentTier(int customerId)
        {
            var points = GetTotalPoints(customerId);
            return _db.RewardTiers.Where(t => t.IsActive && t.PointsRequired <= points)
                .OrderByDescending(t => t.PointsRequired).FirstOrDefault();
        }

        // Claim bonus for current tier
        public int ClaimTierBonus(int customerId)
        {
            var tier = GetCurrentTier(customerId);
            if (tier != null && tier.BonusPoints > 0)
            {
                AwardPoints(customerId, tier.BonusPoints, "TierBonus", $"Claimed bonus for tier {tier.Name}");
                return tier.BonusPoints;
            }
            return 0;
        }

        // Generate promo code (simple random)
        public string GeneratePromoCode(int customerId)
        {
            var code = $"LOYAL-{customerId}-{Guid.NewGuid().ToString()[..8].ToUpper()}";
            // Optionally store promo code in DB
            return code;
        }

        // Get all transactions for a customer
        public List<LoyaltyPointTransaction> GetTransactions(int customerId)
        {
            return _db.LoyaltyPointTransactions.Where(t => t.CustomerId == customerId)
                .OrderByDescending(t => t.Timestamp).ToList();
        }

        // Get all reward tiers
        public List<RewardTier> GetAllTiers()
        {
            return _db.RewardTiers.OrderBy(t => t.PointsRequired).ToList();
        }

        // Add a new reward tier
        public void AddTier(RewardTier tier)
        {
            _db.RewardTiers.Add(tier);
            _db.SaveChanges();
        }

        // Award random bonus points (surprise drop)
        public int TryAwardRandomBonus(int customerId, string trigger = "login")
        {
            var rand = new Random();
            // 10% chance for bonus on trigger
            if (rand.NextDouble() < 0.10)
            {
                int bonus = rand.Next(5, 21); // 5-20 points
                AwardPoints(customerId, bonus, "RandomBonus", $"Surprise bonus for {trigger}");
                // SignalR notification logic will be triggered externally
                return bonus;
            }
            return 0;
        }

        // Expose DbContext for analytics in admin page
        public ApplicationDbContext GetDbContext() => _db;
    }
}
