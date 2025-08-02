using Data;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class BehaviorCenterModel : PageModel
{
    private readonly ApplicationDbContext _db;
    private readonly TechnicianBehaviorAnalyzerService _analyzerService;
    public BehaviorCenterModel(ApplicationDbContext db, TechnicianBehaviorAnalyzerService analyzerService)
    {
        _db = db;
        _analyzerService = analyzerService;
    }

    [BindProperty(SupportsGet = true)] public string TechFilter { get; set; }
    [BindProperty(SupportsGet = true)] public string DateFilter { get; set; }
    [BindProperty(SupportsGet = true)] public string SeverityFilter { get; set; }
    [BindProperty(SupportsGet = true)] public bool AutoFlagEnabled { get; set; }
    // [Sprint1002_FixItFred] Explicitly qualified to resolve ambiguous reference
    public List<Data.Models.TechnicianBehaviorLog> BehaviorLogs { get; set; } = new();
    public string StatusMessage { get; set; }

    public async Task OnGetAsync()
    {
        var query = _db.TechnicianBehaviorLogs.AsQueryable();
        if (!string.IsNullOrEmpty(TechFilter) && int.TryParse(TechFilter, out var techId))
            query = query.Where(l => l.TechnicianId == techId);
        if (!string.IsNullOrEmpty(DateFilter) && DateTime.TryParse(DateFilter, out var dt))
            query = query.Where(l => l.Timestamp.Date == dt.Date);
        if (!string.IsNullOrEmpty(SeverityFilter))
            query = query.Where(l => l.SeverityLevel == SeverityFilter);
        BehaviorLogs = await query.OrderByDescending(l => l.Timestamp).ToListAsync();
    }

    public async Task<IActionResult> OnPostAnalyzePatternsAsync()
    {
        StatusMessage = "Analyzing technician patterns...";
        var logs = await _analyzerService.AnalyzeTechnicianPatternsAsync();
        BehaviorLogs = logs;
        StatusMessage = $"Analysis complete. {logs.Count} patterns detected.";
        if (AutoFlagEnabled)
        {
            await _analyzerService.AutoFlagCriticalBehaviorsAsync();
            StatusMessage += " Critical behaviors auto-flagged.";
        }
        await OnGetAsync();
        return Page();
    }
}
