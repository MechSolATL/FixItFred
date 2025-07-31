using System;
using System.Collections.Generic;
using System.Linq;
using Pages.Technician;
using Data;
using Data.Models;

namespace Services
{
    // Sprint 84.5 — Technician Leaderboard + Rivalry Engine
    public class TechnicianLeaderboardService : ITechnicianLeaderboardService // Sprint 84.7.2 — Live Filter + UI Overlay
    {
        private readonly ApplicationDbContext _db;
        public TechnicianLeaderboardService(ApplicationDbContext db)
        {
            _db = db;
        }

        // Get leaderboard by tier, points, or city
        // Sprint 84.5.1 — Leaderboard Expansion: Add daily delta, rank change, rivalry heat
        public List<TechnicianLeaderboardEntry> GetLeaderboard(string groupBy = "points", int topN = 20)
        {
            var today = DateTime.UtcNow.Date;
            var yesterday = today.AddDays(-1);
            var query = _db.Technicians.AsQueryable();
            switch (groupBy)
            {
                case "tier":
                    query = query.OrderByDescending(t => t.TierLevel);
                    break;
                case "city":
                    query = query.OrderBy(t => t.City);
                    break;
                default:
                    query = query.OrderByDescending(t => t.TotalPoints);
                    break;
            }
            var leaderboard = query.Take(topN).ToList();
            var entries = new List<TechnicianLeaderboardEntry>();
            for (int i = 0; i < leaderboard.Count; i++)
            {
                var t = leaderboard[i];
                // Points gained today
                var pointsToday = _db.TechnicianScoreEntries.Where(e => e.TechnicianId == t.Id && e.Date.Date == today).Sum(e => e.Points);
                // Rank change delta (compare to yesterday's rank)
                var yesterdayPoints = _db.TechnicianScoreEntries.Where(e => e.TechnicianId == t.Id && e.Date.Date == yesterday).Sum(e => e.Points);
                var rankChangeDelta = pointsToday - yesterdayPoints;
                // Rivalry heat level (simple: top 3 in city get highest heat)
                var cityTechs = _db.Technicians.Where(x => x.City == t.City).OrderByDescending(x => x.TotalPoints).Take(3).ToList();
                int rivalryHeatLevel = cityTechs.Any(x => x.Id == t.Id) ? 3 : 1;
                entries.Add(new TechnicianLeaderboardEntry
                {
                    TechnicianId = t.Id,
                    Name = t.Name,
                    TierLevel = t.TierLevel,
                    TotalPoints = t.TotalPoints,
                    City = t.City,
                    PointsGainedToday = pointsToday,
                    RankChangeDelta = rankChangeDelta,
                    RivalryHeatLevel = rivalryHeatLevel
                });
            }
            return entries;
        }

        // Get rivalry stats (anonymous and named)
        public List<TechnicianRivalryStat> GetRivalryStats(int technicianId)
        {
            var tech = _db.Technicians.FirstOrDefault(t => t.Id == technicianId);
            if (tech == null) return new List<TechnicianRivalryStat>();
            var rivals = _db.Technicians.Where(t => t.City == tech.City && t.Id != technicianId).ToList();
            var stats = new List<TechnicianRivalryStat>();
            foreach (var rival in rivals)
            {
                stats.Add(new TechnicianRivalryStat
                {
                    RivalTechnicianId = rival.Id,
                    RivalName = rival.Name,
                    RivalTierLevel = rival.TierLevel,
                    RivalTotalPoints = rival.TotalPoints,
                    DailyDelta = rival.TotalPoints - tech.TotalPoints
                });
            }
            return stats;
        }

        // Calculate daily performance delta
        public int GetDailyPerformanceDelta(int technicianId)
        {
            var today = DateTime.UtcNow.Date;
            var yesterday = today.AddDays(-1);
            var todayPoints = _db.TechnicianScoreEntries.Where(e => e.TechnicianId == technicianId && e.Date.Date == today).Sum(e => e.Points);
            var yesterdayPoints = _db.TechnicianScoreEntries.Where(e => e.TechnicianId == technicianId && e.Date.Date == yesterday).Sum(e => e.Points);
            return todayPoints - yesterdayPoints;
        }

        // Sprint 84.7.2 — Live Filter + UI Overlay
        public List<RewardTier> GetTechnicianTiers()
        {
            return _db.RewardTiers.OrderBy(t => t.PointsRequired).ToList();
        }

        public ReviewsModel.ReviewStats GetReviewStats(int technicianId, string? selectedTier)
        {
            var reviews = _db.CustomerReviews.Where(r => r.TechnicianId == technicianId && r.IsApproved);
            if (!string.IsNullOrEmpty(selectedTier))
                reviews = reviews.Where(r => r.Tier == selectedTier);
            var list = reviews.ToList();
            var avg = list.Any() ? list.Average(r => r.Rating) : 0.0;
            var tierName = selectedTier ?? "All";
            var badgeColor = "#2196f3"; // TODO: Map tier to color if needed
            return new ReviewsModel.ReviewStats
            {
                TierName = tierName,
                BadgeColor = badgeColor,
                AverageRating = avg,
                TotalReviews = list.Count
            };
        }
    }

    public class TechnicianLeaderboardEntry
    {
        public int TechnicianId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int TierLevel { get; set; }
        public int TotalPoints { get; set; }
        public string City { get; set; } = string.Empty;
        // Sprint 84.5.1 — Leaderboard Expansion
        public int PointsGainedToday { get; set; }
        public int RankChangeDelta { get; set; }
        public int RivalryHeatLevel { get; set; }
    }

    public class TechnicianRivalryStat
    {
        public int RivalTechnicianId { get; set; }
        public string RivalName { get; set; } = string.Empty;
        public int RivalTierLevel { get; set; }
        public int RivalTotalPoints { get; set; }
        public int DailyDelta { get; set; }
    }
}
