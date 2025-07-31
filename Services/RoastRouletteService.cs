// Sprint 83.6-RoastRoulette
using MVP_Core.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data;
using Services.Admin;

namespace Services
{
    // Sprint 83.6-RoastRoulette
    public class RoastRouletteService
    {
        private readonly ApplicationDbContext _db;
        private readonly RoastEngineService _roastEngineService;
        public RoastRouletteService(ApplicationDbContext db)
        {
            _db = db;
            _roastEngineService = new RoastEngineService(db);
        }

        // Sprint 83.6-RoastRoulette: Load roast templates by tier
        public async Task<List<RoastTemplate>> GetTemplatesByTierAsync(RoastTier tier)
        {
            return await _db.RoastTemplates.Where(x => x.Tier == tier && !x.Retired).ToListAsync();
        }

        // Sprint 83.6-RoastRoulette: Weighted randomization using delivery metrics
        public async Task<RoastRouletteResult?> GetWeightedRandomRoastAsync(RoastTier tier)
        {
            var templates = await GetTemplatesByTierAsync(tier);
            if (!templates.Any()) return null;
            // Weight by SuccessRate, lower TimesUsed, higher SuccessRate
            var weighted = templates.Select(t => new {
                Template = t,
                Weight = (1.0 + t.SuccessRate) / (1.0 + t.TimesUsed)
            }).ToList();
            var totalWeight = weighted.Sum(w => w.Weight);
            var rand = new Random();
            var pick = rand.NextDouble() * totalWeight;
            double cumulative = 0;
            foreach (var w in weighted)
            {
                cumulative += w.Weight;
                if (pick <= cumulative)
                {
                    // Sentiment and EscalationIndex are stubbed for now
                    return new RoastRouletteResult
                    {
                        RoastTemplateId = w.Template.Id,
                        DeliveryTime = DateTime.UtcNow,
                        Tier = w.Template.Tier,
                        Sentiment = w.Template.Category ?? "Neutral",
                        SuggestedEscalationIndex = (int)w.Template.Tier
                    };
                }
            }
            // fallback
            var t = templates.First();
            return new RoastRouletteResult
            {
                RoastTemplateId = t.Id,
                DeliveryTime = DateTime.UtcNow,
                Tier = t.Tier,
                Sentiment = t.Category ?? "Neutral",
                SuggestedEscalationIndex = (int)t.Tier
            };
        }

        // Sprint 83.6-RoastRoulette: Log delivery
        public async Task LogDeliveryAsync(string userId, int roastTemplateId, string triggeredBy, string deliveryResult)
        {
            var log = new RoastDeliveryLog
            {
                UserId = userId,
                Timestamp = DateTime.UtcNow,
                RoastTemplateId = roastTemplateId,
                TriggeredBy = triggeredBy,
                DeliveryResult = deliveryResult
            };
            _db.RoastDeliveryLogs.Add(log);
            await _db.SaveChangesAsync();
        }
    }

    // Sprint 83.6-RoastRoulette: DTO for roulette result
    public class RoastRouletteResult
    {
        public int RoastTemplateId { get; set; }
        public DateTime DeliveryTime { get; set; }
        public RoastTier Tier { get; set; }
        public string Sentiment { get; set; } = "Neutral";
        public int SuggestedEscalationIndex { get; set; }
    }
}
