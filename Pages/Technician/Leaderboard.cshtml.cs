using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Services;

namespace MVP_Core.Pages.Technician
{
    // Sprint 84.5 — Technician Leaderboard + Rivalry Engine
    public class LeaderboardModel : PageModel
    {
        public List<TechnicianLeaderboardEntry> Leaderboard { get; set; } = new();
        private readonly TechnicianLeaderboardService _leaderboardService;
        public LeaderboardModel(TechnicianLeaderboardService leaderboardService)
        {
            _leaderboardService = leaderboardService;
        }
        public void OnGet()
        {
            Leaderboard = _leaderboardService.GetLeaderboard();
        }
    }
}
