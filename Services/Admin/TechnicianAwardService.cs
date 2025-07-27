using MVP_Core.Data.Models;
using MVP_Core.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace MVP_Core.Services.Admin
{
    public class TechnicianAwardService
    {
        private readonly ApplicationDbContext _db;
        public TechnicianAwardService(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task IssueAwardAsync(int technicianId, string awardType, string? reason, string issuer, int trustBoost, int karmaBoost, string? awardLevel)
        {
            var award = new TechnicianAwardLog
            {
                TechnicianId = technicianId,
                AwardType = awardType,
                Reason = reason,
                Issuer = issuer,
                TrustBoostScore = trustBoost,
                KarmaBoostScore = karmaBoost,
                AwardLevel = awardLevel,
                IssuedAt = DateTime.UtcNow
            };
            _db.TechnicianAwardLogs.Add(award);
            await _db.SaveChangesAsync();
        }
        public async Task<List<TechnicianAwardLog>> GetAwardsAsync(int? technicianId = null, DateTime? from = null, DateTime? to = null, string? awardType = null)
        {
            var query = _db.TechnicianAwardLogs.AsQueryable();
            if (technicianId.HasValue)
                query = query.Where(a => a.TechnicianId == technicianId.Value);
            if (from.HasValue)
                query = query.Where(a => a.IssuedAt >= from.Value);
            if (to.HasValue)
                query = query.Where(a => a.IssuedAt <= to.Value);
            if (!string.IsNullOrWhiteSpace(awardType))
                query = query.Where(a => a.AwardType == awardType);
            return await query.OrderByDescending(a => a.IssuedAt).ToListAsync();
        }
        public int CalculateAwardImpact(TechnicianAwardLog award)
        {
            // Example: trust + karma boost
            return award.TrustBoostScore + award.KarmaBoostScore;
        }
    }
}
