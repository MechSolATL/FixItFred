using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Services.Admin;
using Data;
using Services.HumorOps;

namespace Pages.Admin
{
    public class RoastCenterModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly RoastEngineService _roastEngine;
        private readonly RoastRouletteEngine _rouletteEngine;
        private readonly ILogger<RoastCenterModel> _logger;
        public RoastCenterModel(ApplicationDbContext db, ILogger<RoastCenterModel> logger, RoastRouletteEngine rouletteEngine, RoastFeedbackService roastFeedbackService)
        {
            _db = db;
            _roastEngine = new RoastEngineService(db);
            _rouletteEngine = rouletteEngine;
            _logger = logger;
            RoastFeedbackService = roastFeedbackService;
        }

        public List<NewHireRoastLog> PendingRoasts { get; set; } = new();
        public List<NewHireRoastLog> PastRoasts { get; set; } = new();
        public Dictionary<string, double> RoastRankScores { get; set; } = new();
        public List<NewHireRoastLog> RoastRouletteLog { get; set; } = new();
        public RoastFeedbackService RoastFeedbackService { get; set; }

        public async Task OnGetAsync()
        {
            PendingRoasts = await _db.NewHireRoastLogs.Include(x => x.RoastTemplate).Where(x => !x.IsDelivered).OrderBy(x => x.ScheduledFor).ToListAsync();
            PastRoasts = await _db.NewHireRoastLogs.Include(x => x.RoastTemplate).Where(x => x.IsDelivered).OrderByDescending(x => x.DeliveredAt).ToListAsync();
            RoastRankScores = await _roastEngine.GetRoastRankScoresAsync();
            RoastRouletteLog = await _db.NewHireRoastLogs.Where(x => x.IsDelivered && x.RoastMessage.Contains("Roulette")).OrderByDescending(x => x.DeliveredAt).ToListAsync();
        }

        public async Task<IActionResult> OnPostDeliverAsync(int id)
        {
            await _roastEngine.DeliverRoastAsync(id);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostScheduleAsync(string EmployeeId, RoastTier Tier)
        {
            await _roastEngine.ScheduleRoastAsync(EmployeeId, Tier);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostManualRouletteDropAsync()
        {
            var eligibleTargets = await _rouletteEngine.GetWeightedEligibleTargetsAsync();
            int delivered = 0;
            foreach (var target in eligibleTargets)
            {
                if (delivered >= 10) break;
                var tier = target.RoastTierPreference != null && Enum.TryParse<RoastTier>(target.RoastTierPreference, out var prefTier)
                    ? prefTier : _rouletteEngine.GetNextTier();
                var template = await _rouletteEngine.GetRandomRoastTemplateAsync(tier);
                if (template == null) continue;
                await _rouletteEngine.DropRoastAsync(target, template);
                delivered++;
            }
            _logger.LogInformation($"Manual Roast Roulette Drop: {delivered} roasts delivered.");
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostFeedbackAsync(int id, string reactionType)
        {
            // Log feedback reaction
            await RoastFeedbackService.LogReactionAsync(id, User.Identity?.Name ?? "Anonymous", reactionType);
            // Check for legendary promotion
            await RoastFeedbackService.PromoteRoastToLegendaryTier(id);
            return RedirectToPage();
        }
    }
}
