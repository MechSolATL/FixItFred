using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models;
using Microsoft.EntityFrameworkCore;
using Services.Analytics;
using Data.Models;
using Data;

namespace Services.Mentorship
{
    // Sprint 86.6 — Mentor Selector Engine
    public class MentorSelectorEngine
    {
        private readonly ApplicationDbContext _db;
        private readonly ActionLogService _actionLogService;
        public MentorSelectorEngine(ApplicationDbContext db, ActionLogService actionLogService)
        {
            _db = db;
            _actionLogService = actionLogService;
        }
        public async Task<List<AdminUser>> GetSuggestedMentorsForUser(int requestingUserId)
        {
            var user = await _db.AdminUsers.FindAsync(requestingUserId);
            if (user == null) return new List<AdminUser>();
            // Get all admins with TrustIndex > 85, same JobFunction, and FlowComplianceScore > 90%
            var candidates = await _db.AdminUsers
                .Where(a => a.Id != requestingUserId && a.JobFunction == user.JobFunction && a.TrustIndex > 85)
                .ToListAsync();
            var result = new List<AdminUser>();
            foreach (var candidate in candidates)
            {
                var flowScore = await _actionLogService.GetFlowComplianceScore(candidate.Id);
                if (flowScore >= 90)
                    result.Add(candidate);
            }
            return result.OrderByDescending(a => a.TrustIndex).ToList();
        }
    }
}
