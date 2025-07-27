using MVP_Core.Data;
using MVP_Core.Data.Models;
using MVP_Core.Services.Admin;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVP_Core.Services.HumorOps
{
    public class RoastRouletteEngine
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<RoastRouletteEngine> _logger;
        private readonly MVP_Core.Services.Admin.RoastEngineService _roastEngineService;
        private const int CooldownDays = 10;
        private static readonly RoastTier[] TierRotation = new[] { RoastTier.Soft, RoastTier.Medium, RoastTier.Savage };
        private int _tierIndex = 0;

        public RoastRouletteEngine(ApplicationDbContext db, ILogger<RoastRouletteEngine> logger, MVP_Core.Services.Admin.RoastEngineService roastEngineService)
        {
            _db = db;
            _logger = logger;
            _roastEngineService = roastEngineService;
        }

        // Select eligible targets for roast roulette
        public async Task<List<EmployeeMilestoneLog>> SelectEligibleTargetsAsync()
        {
            var now = DateTime.UtcNow;
            var employees = await _db.EmployeeMilestoneLogs
                .Where(x => x.MilestoneType == "Hire")
                .ToListAsync();
            var eligible = employees
                .Where(x => !x.IsOptedOutOfRoasts)
                .Where(x => !x.LastRoastDeliveredAt.HasValue || (now - x.LastRoastDeliveredAt.Value).TotalDays >= CooldownDays)
                .ToList();
            return eligible;
        }

        // Get a random roast template for a given tier
        public async Task<RoastTemplate?> GetRandomRoastTemplateAsync(RoastTier tier)
        {
            var templates = await _db.RoastTemplates
                .Where(x => x.Tier == tier && x.TimesUsed < x.UseLimit)
                .ToListAsync();
            if (!templates.Any()) return null;
            var rng = new Random();
            return templates[rng.Next(templates.Count)];
        }

        // Drop a roast for a target
        public async Task DropRoastAsync(EmployeeMilestoneLog target, RoastTemplate template)
        {
            template.TimesUsed++;
            target.LastRoastDeliveredAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            // Log the roast delivery
            _db.NewHireRoastLogs.Add(new NewHireRoastLog
            {
                EmployeeId = target.EmployeeId,
                RoastMessage = template.Message,
                ScheduledFor = DateTime.UtcNow,
                IsDelivered = true,
                DeliveredAt = DateTime.UtcNow,
                RoastLevel = (int)template.Tier
            });
            await _db.SaveChangesAsync();
            _logger.LogInformation($"Roast delivered to {target.EmployeeId} with tier {template.Tier}");
        }

        // Enforce cooldown
        public bool EnforceCooldown(EmployeeMilestoneLog target)
        {
            if (!target.LastRoastDeliveredAt.HasValue) return true;
            return (DateTime.UtcNow - target.LastRoastDeliveredAt.Value).TotalDays >= CooldownDays;
        }

        // Weighted selection based on RoastRankScore
        public async Task<List<EmployeeMilestoneLog>> GetWeightedEligibleTargetsAsync()
        {
            var eligible = await SelectEligibleTargetsAsync();
            var scores = await _roastEngineService.GetRoastRankScoresAsync();
            // Lower scores get higher chance
            var weighted = eligible.OrderBy(x => scores.ContainsKey(x.EmployeeId) ? scores[x.EmployeeId] : 0).ToList();
            return weighted;
        }

        // Rotate roast tier
        public RoastTier GetNextTier()
        {
            var tier = TierRotation[_tierIndex % TierRotation.Length];
            _tierIndex++;
            return tier;
        }
    }
}
