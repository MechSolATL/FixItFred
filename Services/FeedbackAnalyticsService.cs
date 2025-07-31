using Data;
using Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Services
{
    // Sprint 57.0: Feedback Analytics & Sentiment Insights
    public class FeedbackAnalyticsService
    {
        private readonly ApplicationDbContext _db;
        public FeedbackAnalyticsService(ApplicationDbContext db)
        {
            _db = db;
        }

        // Analyze sentiment: returns -1 (negative), 0 (neutral), 1 (positive)
        public float CalculateSentiment(string feedback)
        {
            if (string.IsNullOrWhiteSpace(feedback)) return 0;
            var negativeWords = new[] { "bad", "poor", "terrible", "slow", "rude", "unhappy", "issue", "problem", "disappointed" };
            var positiveWords = new[] { "great", "excellent", "fast", "friendly", "happy", "amazing", "helpful", "satisfied", "awesome" };
            int score = 0;
            foreach (var word in positiveWords)
                if (feedback.IndexOf(word, StringComparison.OrdinalIgnoreCase) >= 0) score++;
            foreach (var word in negativeWords)
                if (feedback.IndexOf(word, StringComparison.OrdinalIgnoreCase) >= 0) score--;
            return Math.Clamp(score, -3, 3) / 3f; // Normalize to -1..1
        }

        // Extract keywords (simple): returns comma-separated keywords
        public string ExtractKeywords(string feedback)
        {
            if (string.IsNullOrWhiteSpace(feedback)) return "";
            var words = Regex.Matches(feedback.ToLower(), "[a-zA-Z]{4,}")
                .Select(m => m.Value)
                .GroupBy(w => w)
                .OrderByDescending(g => g.Count())
                .Take(5)
                .Select(g => g.Key)
                .ToList();
            return string.Join(", ", words);
        }

        // Flag review if concerning phrases found
        public bool IsConcerning(string feedback)
        {
            if (string.IsNullOrWhiteSpace(feedback)) return false;
            var concerning = new[] { "unsafe", "fraud", "damage", "complaint", "problem", "issue", "refund", "angry", "cancel" };
            return concerning.Any(w => feedback.IndexOf(w, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        // Analyze and update all reviews
        public void AnalyzeAllReviews()
        {
            var reviews = _db.CustomerReviews.ToList();
            foreach (var review in reviews)
            {
                review.SentimentScore = CalculateSentiment(review.Feedback ?? "");
                review.Keywords = ExtractKeywords(review.Feedback ?? "");
                review.IsFlagged = IsConcerning(review.Feedback ?? "");
            }
            _db.SaveChanges();
        }

        // Group by technician
        public Dictionary<int, List<CustomerReview>> GetReviewsByTechnician()
        {
            return _db.CustomerReviews.GroupBy(r => r.ServiceRequestId)
                .ToDictionary(g => g.Key, g => g.ToList());
        }

        // Get flagged reviews
        public List<CustomerReview> GetFlaggedReviews()
        {
            return _db.CustomerReviews.Where(r => r.IsFlagged).ToList();
        }

        // Get all reviews
        public List<CustomerReview> GetAllReviews()
        {
            return _db.CustomerReviews.ToList();
        }
    }
}
