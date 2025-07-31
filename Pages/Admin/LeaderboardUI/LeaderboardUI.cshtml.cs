using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Services.Leaderboard;
using MVP_Core.Services.Leaderboard.Models;
using System.Collections.Generic;
using System.Linq;

public class LeaderboardUIModel : PageModel
{
    private readonly ILeaderboardService _service;
    public List<LeaderboardEntry> Leaderboard { get; set; } = new();

    public LeaderboardUIModel(ILeaderboardService service)
    {
        _service = service;
    }

    public void OnGet()
    {
        Leaderboard = _service.GetLeaderboard().OrderByDescending(e => e.CompletedJobs).ToList();
    }
}