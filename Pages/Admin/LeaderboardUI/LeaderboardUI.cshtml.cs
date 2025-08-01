using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Data.Models;
using MVP_Core.Extensions;
using MVP_Core.Services.Leaderboard;
using MVP_Core.Services.Leaderboard.Models;
using System.Collections.Generic;
using System.Linq;

public class LeaderboardUIModel : PageModel
{
    private readonly ILeaderboardService _service;
    
    [BindProperty]
    public List<LeaderboardEntry> Leaderboard { get; set; } = new();

    public LeaderboardUIModel(ILeaderboardService service)
    {
        _service = service;
    }

    [ValidateAntiForgeryToken]
    public void OnGet()
    {
        Leaderboard = _service.GetLeaderboard().OrderByDescending(e => e.CompletedJobs).ToList();
        
        // Sprint93_04 — FixItFredReplay binding
        FixItFredExtension.ReplayLastInjectedModules();
    }
}