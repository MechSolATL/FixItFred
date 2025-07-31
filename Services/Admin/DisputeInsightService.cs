using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Data;
using Data.Models;

namespace Services.Admin
{
    // Sprint 70.5: Dispute Insight Service
    public class DisputeInsightService
    {
        private readonly ApplicationDbContext _db;
        public DisputeInsightService(ApplicationDbContext db)
        {
            _db = db;
        }
        // Log a dispute insight
        public async Task LogInsightAsync(int disputeId, string type, string description, string user)
        {
            var log = new DisputeInsightLog
            {
                DisputeId = disputeId,
                InsightType = type,
                Description = description,
                LoggedBy = user,
                Timestamp = DateTime.UtcNow
            };
            _db.DisputeInsightLogs.Add(log);
            await _db.SaveChangesAsync();
        }
        // Get all insights for a dispute
        public async Task<List<DisputeInsightLog>> GetInsightsForDisputeAsync(int disputeId)
        {
            return await _db.DisputeInsightLogs
                .AsNoTracking()
                .Where(x => x.DisputeId == disputeId)
                .OrderByDescending(x => x.Timestamp)
                .ToListAsync();
        }
    }
}
