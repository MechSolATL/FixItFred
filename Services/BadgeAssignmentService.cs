using MVP_Core.Data;
using MVP_Core.Data.Models.PatchAnalytics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVP_Core.Services
{
    // Sprint 91.22.4 - BadgeAssignment
    public class BadgeAssignmentService
    {
        private readonly ApplicationDbContext _db;
        public BadgeAssignmentService(ApplicationDbContext db)
        {
            _db = db;
        }

        // Assign badges for the current week
        public async Task AssignWeeklyBadgesAsync()
        {
            var now = DateTime.UtcNow;
            var weekAgo = now.AddDays(-7);
            var fourWeeksAgo = now.AddDays(-28);

            // TopShout: ?3 Patch mentions in a week
            var shoutCounts = _db.Set<PatchPostLog>()
                .Where(p => p.ContentType == "ShoutOut" && p.Timestamp >= weekAgo && p.TechnicianId != null)
                .GroupBy(p => p.TechnicianId!.Value)
                .Select(g => new { TechnicianId = g.Key, Count = g.Count() })
                .Where(x => x.Count >= 3)
                .ToList();
            foreach (var shout in shoutCounts)
            {
                await AwardBadgeAsync(shout.TechnicianId, "TopShout", "TopShout — For being featured 3+ times in Patch shout-outs this week.", "/assets/badges/TopShout.png");
            }

            // PatchFav: Quote of the Week
            var topQuote = _db.Set<PatchQuoteMemory>().OrderByDescending(q => q.UsageCount).FirstOrDefault();
            if (topQuote != null && topQuote.TriggeredByTechnicianId.HasValue)
            {
                await AwardBadgeAsync(topQuote.TriggeredByTechnicianId.Value, "PatchFav", "Patch’s Favorite — Best caption of the week, you slick legend.", "/assets/badges/PatchFav.png");
            }

            // PromoKing: ?5 promo impacts in 7 days
            var promoCounts = _db.Set<PromoImpactScore>()
                .Where(p => p.LastMentioned >= weekAgo && p.TechnicianId != null)
                .GroupBy(p => p.TechnicianId!.Value)
                .Select(g => new { TechnicianId = g.Key, Count = g.Sum(x => x.MentionCount) })
                .Where(x => x.Count >= 5)
                .ToList();
            foreach (var promo in promoCounts)
            {
                await AwardBadgeAsync(promo.TechnicianId, "PromoKing", "Promo King — That deal post hit like a furnace in July.", "/assets/badges/PromoKing.png");
            }

            // StreakMaster: Consistent Patch export activity over 4 weeks
            var streakTechs = _db.Set<PatchPostLog>()
                .Where(p => p.Timestamp >= fourWeeksAgo && p.TechnicianId != null)
                .GroupBy(p => p.TechnicianId!.Value)
                .Where(g => g.Count() >= 1) // At least 1 export per week
                .Select(g => new { TechnicianId = g.Key, Weeks = g.Select(x => x.Timestamp.Date.AddDays(-(int)x.Timestamp.DayOfWeek)).Distinct().Count() })
                .Where(x => x.Weeks >= 4)
                .ToList();
            foreach (var streak in streakTechs)
            {
                await AwardBadgeAsync(streak.TechnicianId, "StreakMaster", "StreakMaster — Consistent Patch export activity for 4+ weeks.", "/assets/badges/StreakMaster.png");
            }

            await _db.SaveChangesAsync();
        }

        // Award badge if not already earned this week
        private async Task AwardBadgeAsync(int technicianId, string badgeType, string description, string iconPath)
        {
            var weekAgo = DateTime.UtcNow.AddDays(-7);
            bool alreadyAwarded = _db.Set<TechnicianBadge>().Any(b => b.TechnicianId == technicianId && b.BadgeType == badgeType && b.EarnedOn >= weekAgo);
            if (!alreadyAwarded)
            {
                var badge = new TechnicianBadge
                {
                    TechnicianId = technicianId,
                    BadgeType = badgeType,
                    EarnedOn = DateTime.UtcNow,
                    Description = description,
                    IconPath = iconPath
                };
                _db.Add(badge);
                await _db.SaveChangesAsync();
            }
        }

        // Get all badges for a technician
        public async Task<List<TechnicianBadge>> GetBadgesForTechnicianAsync(int technicianId)
        {
            return await Task.FromResult(_db.Set<TechnicianBadge>().Where(b => b.TechnicianId == technicianId).OrderByDescending(b => b.EarnedOn).ToList());
        }
    }
}
