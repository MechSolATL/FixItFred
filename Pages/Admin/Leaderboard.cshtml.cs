using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace MVP_Core.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class LeaderboardModel : PageModel
    {
        public List<LeaderboardEntryViewModel> Leaderboard { get; set; } = new();
        public string Filter { get; set; } = "all";

        public async Task OnGetAsync(string? filter)
        {
            Filter = filter ?? "all";
            // TODO: Replace with real DB/service logic
            var allEntries = await Task.FromResult(GetSampleLeaderboard());
            Leaderboard = Filter switch
            {
                "monthly" => allEntries.OrderByDescending(e => e.JobsCompleted).Take(10).ToList(),
                "weekly" => allEntries.OrderByDescending(e => e.JobsCompleted).Take(5).ToList(),
                _ => allEntries.OrderByDescending(e => e.JobsCompleted).ToList()
            };
        }

        // Sample/mock data for demo
        private List<LeaderboardEntryViewModel> GetSampleLeaderboard() => new()
        {
            new LeaderboardEntryViewModel { Rank = 1, Name = "Alex Pro", JobsCompleted = 120, Rating = 4.9, UpsellCount = 15, Badges = ["PROS", "Certified"], RankChange = 1 },
            new LeaderboardEntryViewModel { Rank = 2, Name = "Jamie TopSeller", JobsCompleted = 110, Rating = 4.8, UpsellCount = 22, Badges = ["Top Seller"], RankChange = 0 },
            new LeaderboardEntryViewModel { Rank = 3, Name = "Sam Cert", JobsCompleted = 105, Rating = 4.7, UpsellCount = 10, Badges = ["Certified"], RankChange = -1 },
        };

        public record LeaderboardEntryViewModel
        {
            public int Rank { get; init; }
            public string Name { get; init; } = string.Empty;
            public int JobsCompleted { get; init; }
            public double Rating { get; init; }
            public int UpsellCount { get; init; }
            public List<string> Badges { get; init; } = new();
            public int RankChange { get; init; } // +1, 0, -1
        }
    }
}
