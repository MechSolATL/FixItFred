using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Services;
using System.Collections.Generic;

namespace MVP_Core.Pages.Technician
{
    // Sprint 84.5.1 — Razor Rename: Leaderboard.cshtml to LeaderboardView.cshtml (encoding fix)
    public class LeaderboardViewModel : PageModel
    {
        public List<TechnicianLeaderboardEntry> Leaderboard { get; set; } = new();
        private readonly TechnicianLeaderboardService _leaderboardService;
        public LeaderboardViewModel(TechnicianLeaderboardService leaderboardService)
        {
            _leaderboardService = leaderboardService;
        }
        public void OnGet()
        {
            Leaderboard = _leaderboardService.GetLeaderboard();
        }
    }
}
