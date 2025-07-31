using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using Data.Models;
using Services;

namespace Pages
{
    /// <summary>
    /// PageModel for Rewards page. Displays loyalty status, transactions, and reviews.
    /// </summary>
    public class RewardsModel : PageModel
    {
        private readonly LoyaltyRewardService _loyaltyService;
        private readonly ReviewService _reviewService;
        public int CustomerId { get; set; }
        public int TotalPoints { get; set; }
        public RewardTier? CurrentTier { get; set; }
        public List<LoyaltyPointTransaction> Transactions { get; set; } = new();
        public List<CustomerReview> Reviews { get; set; } = new();

        public RewardsModel(LoyaltyRewardService loyaltyService, ReviewService reviewService)
        {
            _loyaltyService = loyaltyService;
            _reviewService = reviewService;
        }

        public void OnGet()
        {
            // TODO: Replace with actual customer ID logic
            CustomerId = User?.Identity?.Name != null ? int.Parse(User.Identity.Name) : 0;
            TotalPoints = _loyaltyService.GetTotalPoints(CustomerId);
            CurrentTier = _loyaltyService.GetCurrentTier(CustomerId);
            Transactions = _loyaltyService.GetTransactions(CustomerId);
            Reviews = _reviewService.GetReviews(CustomerId);
        }
    }
}
