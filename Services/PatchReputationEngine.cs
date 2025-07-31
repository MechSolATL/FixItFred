using Data;
using Data.Models.PatchAnalytics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Services
{
    public class PatchReputationEngine
    {
        private readonly ApplicationDbContext _db;
        public PatchReputationEngine(ApplicationDbContext db)
        {
            _db = db;
        }

        // Log a Patch action (caption, tag, shout-out, etc.)
        public void LogPatchPost(string contentType, string caption, string[] hashtags, int? technicianId)
        {
            var log = new PatchPostLog
            {
                ContentType = contentType,
                CaptionUsed = caption,
                Hashtags = hashtags,
                TechnicianId = technicianId,
                Timestamp = DateTime.UtcNow
            };
            _db.Add(log);
            _db.SaveChanges();
        }

        // Score a technician for Patch mention
        public void AwardPatchPoints(int technicianId, int points)
        {
            var tech = _db.Technicians.FirstOrDefault(t => t.Id == technicianId);
            if (tech != null)
            {
                tech.PatchReputationScore = (tech.PatchReputationScore.HasValue ? tech.PatchReputationScore.Value : 0) + points;
                _db.SaveChanges();
            }
        }

        // Archive a quote if it went viral
        public void ArchiveQuote(string quote, int? triggeredByTechId)
        {
            var memory = _db.Set<PatchQuoteMemory>().FirstOrDefault(q => q.Quote == quote);
            if (memory == null)
            {
                memory = new PatchQuoteMemory
                {
                    Quote = quote,
                    TriggeredByTechnicianId = triggeredByTechId,
                    UsageCount = 1,
                    LastUsed = DateTime.UtcNow
                };
                _db.Add(memory);
            }
            else
            {
                memory.UsageCount++;
                memory.LastUsed = DateTime.UtcNow;
            }
            _db.SaveChanges();
        }

        // Add or update promo tag reference
        public void LogPromoImpact(string promoTag, int? technicianId)
        {
            var promo = _db.Set<PromoImpactScore>().FirstOrDefault(p => p.PromoTag == promoTag);
            if (promo == null)
            {
                promo = new PromoImpactScore
                {
                    PromoTag = promoTag,
                    MentionCount = 1,
                    TechnicianId = technicianId,
                    LastMentioned = DateTime.UtcNow
                };
                _db.Add(promo);
            }
            else
            {
                promo.MentionCount++;
                promo.LastMentioned = DateTime.UtcNow;
            }
            _db.SaveChanges();
        }

        // Leaderboard: Top techs, quotes, hashtags, promo reach
        public List<(int TechnicianId, int Score)> GetTopTechs(int topN = 5)
        {
            return _db.Technicians
                .Where(t => t.PatchReputationScore != null)
                .OrderByDescending(t => t.PatchReputationScore)
                .Take(topN)
                .Select(t => (t.Id, t.PatchReputationScore ?? 0))
                .ToList();
        }

        public PatchQuoteMemory? GetTopQuote()
        {
            return _db.Set<PatchQuoteMemory>().OrderByDescending(q => q.UsageCount).FirstOrDefault();
        }

        public List<string> GetMostUsedHashtags(int topN = 5)
        {
            return _db.Set<PatchPostLog>()
                .SelectMany(p => p.Hashtags)
                .GroupBy(h => h)
                .OrderByDescending(g => g.Count())
                .Take(topN)
                .Select(g => g.Key)
                .ToList();
        }

        public int GetPromoReachEstimate()
        {
            return _db.Set<PromoImpactScore>().Sum(p => p.MentionCount);
        }

        public List<PatchPostLog> GetPatchPostHistory(int count = 20)
        {
            return _db.Set<PatchPostLog>().OrderByDescending(p => p.Timestamp).Take(count).ToList();
        }
    }
}
