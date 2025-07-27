using MVP_Core.Data;
using MVP_Core.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MVP_Core.Services
{
    // Sprint 84.5 — Technician Leaderboard + Rivalry Engine
    public class TechnicianLeaderboardService
    {
        private readonly ApplicationDbContext _db;
        public TechnicianLeaderboardService(ApplicationDbContext db)
        {
            _db = db;
        }

        // Get leaderboard by tier, points, or city
        public List<TechnicianLeaderboardEntry> GetLeaderboard(string groupBy = "points", int topN = 20)
        {
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
            return query.Take(topN).Select(t => new TechnicianLeaderboardEntry
            {
                TechnicianId = t.Id,
                Name = t.Name,
                TierLevel = t.TierLevel,
                TotalPoints = t.TotalPoints,
                City = t.City
            }).ToList();
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
    }

    public class TechnicianLeaderboardEntry
    {
        public int TechnicianId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int TierLevel { get; set; }
        public int TotalPoints { get; set; }
        public string City { get; set; } = string.Empty;
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
