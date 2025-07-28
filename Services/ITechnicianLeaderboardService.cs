using MVP_Core.Data.Models;
using System.Collections.Generic;
using MVP_Core.Pages.Technician; // Sprint 84.7.2 — Live Filter + UI Overlay

namespace MVP_Core.Services
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
