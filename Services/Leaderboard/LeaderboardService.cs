using MVP_Core.Services.Leaderboard.Models;
using System.Collections.Generic;

namespace MVP_Core.Services.Leaderboard
{
    public class LeaderboardService : ILeaderboardService
    {
        public List<LeaderboardEntry> GetLeaderboard()
        {
            return new List<LeaderboardEntry>
            {
                new LeaderboardEntry { Name = "Alex J.", CompletedJobs = 42, Rating = 4.8 },
                new LeaderboardEntry { Name = "Morgan S.", CompletedJobs = 37, Rating = 4.6 },
                new LeaderboardEntry { Name = "Taylor R.", CompletedJobs = 29, Rating = 4.9 }
            };
        }
    }
}