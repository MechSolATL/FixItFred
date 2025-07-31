using System.Collections.Generic;
using Pages.Technician;
using Data.Models;

namespace Services
{
    // Sprint 84.7.2 — Live Filter + UI Overlay
    public interface ITechnicianLeaderboardService
    {
        List<TechnicianLeaderboardEntry> GetLeaderboard(string groupBy = "points", int topN = 20);
        List<TechnicianRivalryStat> GetRivalryStats(int technicianId);
        int GetDailyPerformanceDelta(int technicianId);
        List<RewardTier> GetTechnicianTiers();
        ReviewsModel.ReviewStats GetReviewStats(int technicianId, string? selectedTier); // Fix type reference
    }
}
