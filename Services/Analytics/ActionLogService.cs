using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Models;
using Data;

namespace Services.Analytics
{
    // Sprint 86.5 — Action Log Service for Flow Analytics
    public class ActionLogService
    {
        private readonly ApplicationDbContext _db;
        public ActionLogService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task LogActionAsync(UserActionLog log)
        {
            _db.UserActionLogs.Add(log);
            await _db.SaveChangesAsync();
        }

        public async Task<List<UserActionLog>> GetLogsByUser(int userId)
        {
            return await _db.UserActionLogs.Where(l => l.AdminUserId == userId).ToListAsync();
        }

        public async Task<int> GetCompliantFlowCount(int userId)
        {
            // Example: count sessions with no skips or errors
            return await _db.UserActionLogs
                .Where(l => l.AdminUserId == userId && l.ActionType == "FlowCompleted" && !l.IsError)
                .CountAsync();
        }

        public async Task<int> GetDeviantFlowCount(int userId)
        {
            // Example: count sessions with skips or errors
            return await _db.UserActionLogs
                .Where(l => l.AdminUserId == userId && l.ActionType == "FlowDeviation")
                .CountAsync();
        }

        public async Task<DateTime?> GetLastDeviation(int userId)
        {
            return await _db.UserActionLogs
                .Where(l => l.AdminUserId == userId && l.ActionType == "FlowDeviation")
                .OrderByDescending(l => l.Timestamp)
                .Select(l => (DateTime?)l.Timestamp)
                .FirstOrDefaultAsync();
        }

        public async Task<List<int>> DetectMentorCandidates()
        {
            // 90%+ compliance, 7+ complete flows, no skips
            var userStats = await _db.UserActionLogs
                .GroupBy(l => l.AdminUserId)
                .Select(g => new
                {
                    UserId = g.Key,
                    TotalFlows = g.Count(l => l.ActionType == "FlowCompleted"),
                    Deviations = g.Count(l => l.ActionType == "FlowDeviation"),
                    Compliant = g.Count(l => l.ActionType == "FlowCompleted" && !l.IsError)
                })
                .ToListAsync();
            return userStats
                .Where(s => s.TotalFlows >= 7 && s.Compliant >= 0.9 * (s.TotalFlows + s.Deviations))
                .Select(s => s.UserId)
                .ToList();
        }

        public async Task<List<UserActionLog>> CaptureRecentFlowAsync(int userId, int steps = 15)
        {
            return await _db.UserActionLogs
                .Where(l => l.AdminUserId == userId)
                .OrderByDescending(l => l.Timestamp)
                .Take(steps)
                .OrderBy(l => l.Timestamp)
                .ToListAsync();
        }

        public async Task<int> GetFlowComplianceScore(int userId)
        {
            var total = await _db.UserActionLogs.CountAsync(l => l.AdminUserId == userId && (l.ActionType == "FlowCompleted" || l.ActionType == "FlowDeviation"));
            if (total == 0) return 0;
            var compliant = await _db.UserActionLogs.CountAsync(l => l.AdminUserId == userId && l.ActionType == "FlowCompleted" && !l.IsError);
            return (int)(100.0 * compliant / total);
        }
    }
}
