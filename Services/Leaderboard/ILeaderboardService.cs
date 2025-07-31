using MVP_Core.Services.Leaderboard.Models;
using System.Collections.Generic;

namespace MVP_Core.Services.Leaderboard
{
    public interface ILeaderboardService
    {
        List<LeaderboardEntry> GetLeaderboard();
    }
}