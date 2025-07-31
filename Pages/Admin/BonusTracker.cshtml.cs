using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Data.Models;
using System.Collections.Generic;
using System.Linq;
using Data;
using Services;

namespace Pages.Admin
{
    public class BonusTrackerModel : PageModel
    {
        private readonly TechnicianBonusService _bonusService;
        private readonly ApplicationDbContext _db;
        public List<LeaderboardEntry> Leaderboard { get; set; } = new();
        public List<BonusChartEntry> BonusChart { get; set; } = new();
        public BonusTrackerModel(TechnicianBonusService bonusService, ApplicationDbContext db)
        {
            _bonusService = bonusService;
            _db = db;
        }
        public void OnGet()
        {
            var bonuses = _db.TechnicianBonusLogs.ToList();
            var techs = _db.Technicians.ToList();
            Leaderboard = bonuses.GroupBy(b => b.TechnicianId)
                .Select((g, i) => new LeaderboardEntry
                {
                    Rank = i + 1,
                    Name = techs.FirstOrDefault(t => t.Id == g.Key)?.FullName ?? $"Tech {g.Key}",
                    TotalBonuses = g.Count(),
                    TotalPayout = g.Sum(b => b.Amount)
                })
                .OrderByDescending(e => e.TotalPayout)
                .Take(10)
                .ToList();
            BonusChart = bonuses.GroupBy(b => b.BonusType)
                .Select(g => new BonusChartEntry
                {
                    BonusType = g.Key.ToString(),
                    Count = g.Count(),
                    TotalPayout = g.Sum(b => b.Amount)
                })
                .ToList();
        }
        public class LeaderboardEntry
        {
            public int Rank { get; set; }
            public string Name { get; set; } = "";
            public int TotalBonuses { get; set; }
            public decimal TotalPayout { get; set; }
        }
        public class BonusChartEntry
        {
            public string BonusType { get; set; } = "";
            public int Count { get; set; }
            public decimal TotalPayout { get; set; }
        }
    }
}
