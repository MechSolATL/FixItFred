using MVP_Core.Data.Models;
using System.Collections.Generic;
using System;

namespace MVP_Core.Services
{
    // Sprint 84.7.2 — Live Filter + UI Overlay
    public interface IReviewService
    {
        CustomerReview SubmitReview(int customerId, int serviceRequestId, int rating, string? feedback, bool isBonus = false, string? badge = null);
        List<CustomerReview> GetReviews(int customerId);
        double GetAverageScore(int customerId);
        List<(int CustomerId, int ReviewCount, double AvgScore)> GetMonthlyLeaderboard(DateTime month); // Fix DateTime import
        List<CustomerReview> GetApprovedReviewsByTechnician(int technicianId);
    }
}
