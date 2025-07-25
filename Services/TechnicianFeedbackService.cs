using MVP_Core.Data.Models;
using System.Collections.Generic;
using System.Linq;
using System;

namespace MVP_Core.Services
{
    /// <summary>
    /// TechnicianFeedbackService: Handles feedback submission, retrieval, and rating calculations for technicians.
    /// Recent jobs are weighted more heavily in average rating.
    /// </summary>
    public class TechnicianFeedbackService
    {
        private readonly ApplicationDbContext _db;
        public TechnicianFeedbackService(ApplicationDbContext db)
        {
            _db = db;
        }
        /// <summary>
        /// Submit feedback for a technician/job. Prevents duplicate for same request/tech.
        /// </summary>
        public bool SubmitFeedback(int technicianId, int requestId, int rating, string notes, string submittedBy)
        {
            if (_db.TechnicianFeedbacks.Any(f => f.TechnicianId == technicianId && f.RequestId == requestId))
                return false; // Prevent duplicate
            var feedback = new TechnicianFeedback
            {
                TechnicianId = technicianId,
                RequestId = requestId,
                Rating = rating,
                Notes = notes,
                SubmittedBy = submittedBy,
                SubmittedAt = DateTime.UtcNow
            };
            _db.TechnicianFeedbacks.Add(feedback);
            _db.SaveChanges();
            return true;
        }
        /// <summary>
        /// Get all feedback entries for a technician.
        /// </summary>
        public List<TechnicianFeedback> GetFeedbackForTechnician(int technicianId)
        {
            return _db.TechnicianFeedbacks
                .Where(f => f.TechnicianId == technicianId)
                .OrderByDescending(f => f.SubmittedAt)
                .ToList();
        }
        /// <summary>
        /// Calculate average rating for a technician, weighting recent jobs higher.
        /// </summary>
        public double CalculateAverageRating(int technicianId)
        {
            var feedbacks = GetFeedbackForTechnician(technicianId);
            if (!feedbacks.Any()) return 0;
            // Weight: last 5 jobs x2, older x1
            var recent = feedbacks.Take(5).ToList();
            var older = feedbacks.Skip(5).ToList();
            double weightedSum = recent.Sum(f => f.Rating) * 2 + older.Sum(f => f.Rating);
            int weightedCount = recent.Count * 2 + older.Count;
            return weightedCount > 0 ? Math.Round(weightedSum / weightedCount, 2) : 0;
        }
    }
}
