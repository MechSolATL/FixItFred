using System;
using System.Linq;
using System.Collections.Generic;
using Data;
using Data.Models;

namespace Services
{
    /// <summary>
    /// Handles customer review/rating/questionnaire feedback logic.
    /// </summary>
    public class ReviewService : IReviewService // Sprint 84.7.2 — Live Filter + UI Overlay
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
                SubmittedAt = DateTime.UtcNow,
                IsApproved = true // Sprint 84.7.2 — Live Filter + UI Overlay
            };
            _db.CustomerReviews.Add(review);
            _db.SaveChanges();
            return review;
        }

        // Get reviews for a customer
        public List<CustomerReview> GetReviews(int customerId)
        {
            return _db.CustomerReviews.Where(r => r.CustomerId == customerId)
                .OrderByDescending(r => r.SubmittedAt)
                .Select(r => new CustomerReview
                {
                    Id = r.Id,
                    CustomerId = r.CustomerId,
                    ServiceRequestId = r.ServiceRequestId,
                    TechnicianId = r.TechnicianId,
                    Rating = r.Rating,
                    Feedback = r.Feedback,
                    SubmittedAt = r.SubmittedAt,
                    IsPublic = r.IsPublic,
                    IsGamifiedBonus = r.IsGamifiedBonus,
                    BadgeAwarded = r.BadgeAwarded,
                    SentimentScore = r.SentimentScore,
                    Keywords = r.Keywords,
                    IsFlagged = r.IsFlagged,
                    IsApproved = r.IsApproved,
                    Tier = r.Tier,
                    CustomerName = r.CustomerName,
                    Comment = r.Comment,
                })
                .ToList();
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

        // Sprint 84.7.2 — Live Filter + UI Overlay
        public List<CustomerReview> GetApprovedReviewsByTechnician(int technicianId)
        {
            return _db.CustomerReviews
                .Where(r => r.TechnicianId == technicianId && r.IsApproved)
                .OrderByDescending(r => r.SubmittedAt)
                .Select(r => new CustomerReview
                {
                    Id = r.Id,
                    CustomerId = r.CustomerId,
                    ServiceRequestId = r.ServiceRequestId,
                    TechnicianId = r.TechnicianId,
                    Rating = r.Rating,
                    Feedback = r.Feedback,
                    SubmittedAt = r.SubmittedAt,
                    IsPublic = r.IsPublic,
                    IsGamifiedBonus = r.IsGamifiedBonus,
                    BadgeAwarded = r.BadgeAwarded,
                    SentimentScore = r.SentimentScore,
                    Keywords = r.Keywords,
                    IsFlagged = r.IsFlagged,
                    IsApproved = r.IsApproved,
                    Tier = r.Tier,
                    CustomerName = r.CustomerName,
                    Comment = r.Comment,
                })
                .ToList();
        }
    }
}
