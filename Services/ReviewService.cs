using MVP_Core.Data.Models;
using MVP_Core.Data;
using System;
using System.Linq;
using System.Collections.Generic;

namespace MVP_Core.Services
{
    /// <summary>
    /// Handles customer review/rating/questionnaire feedback logic.
    /// </summary>
    public class ReviewService
    {
        private readonly ApplicationDbContext _db;
        public ReviewService(ApplicationDbContext db)
        {
            _db = db;
        }

        // Submit a review
        public CustomerReview SubmitReview(int customerId, int serviceRequestId, int rating, string? feedback, bool isBonus = false, string? badge = null)
        {
            var review = new CustomerReview
            {
                CustomerId = customerId,
                ServiceRequestId = serviceRequestId,
                Rating = rating,
                Feedback = feedback,
                IsGamifiedBonus = isBonus,
                BadgeAwarded = badge,
                SubmittedAt = DateTime.UtcNow
            };
            _db.CustomerReviews.Add(review);
            _db.SaveChanges();
            return review;
        }

        // Get reviews for a customer
        public List<CustomerReview> GetReviews(int customerId)
        {
            return _db.CustomerReviews.Where(r => r.CustomerId == customerId)
                .OrderByDescending(r => r.SubmittedAt).ToList();
        }

        // Get average review score for a customer
        public double GetAverageScore(int customerId)
        {
            var reviews = _db.CustomerReviews.Where(r => r.CustomerId == customerId);
            return reviews.Any() ? reviews.Average(r => r.Rating) : 0.0;
        }

        // Get leaderboard for top reviewers (monthly)
        public List<(int CustomerId, int ReviewCount, double AvgScore)> GetMonthlyLeaderboard(DateTime month)
        {
            var start = new DateTime(month.Year, month.Month, 1);
            var end = start.AddMonths(1);
            var query = _db.CustomerReviews.Where(r => r.SubmittedAt >= start && r.SubmittedAt < end)
                .GroupBy(r => r.CustomerId)
                .Select(g => new { CustomerId = g.Key, ReviewCount = g.Count(), AvgScore = g.Average(r => r.Rating) })
                .OrderByDescending(x => x.ReviewCount).Take(10);
            return query.ToList().Select(x => (x.CustomerId, x.ReviewCount, x.AvgScore)).ToList();
        }
    }
}
