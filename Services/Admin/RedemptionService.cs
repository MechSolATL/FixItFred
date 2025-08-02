using Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using Data;

namespace Services.Admin
{
    public class RedemptionService
    {
        private readonly ApplicationDbContext _db;
        public RedemptionService(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<List<TechnicianRedemptionLog>> GetRedemptionEntriesAsync()
        {
            return await _db.TechnicianRedemptionLogs
                .OrderByDescending(r => r.Timestamp)
                .ToListAsync();
        }
        public async Task LogRedemptionAsync(int technicianId, int previousTrustScore, int restoredTrustScore, string actionsTaken)
        {
            var entry = new TechnicianRedemptionLog
            {
                TechnicianId = technicianId,
                PreviousTrustScore = previousTrustScore,
                RestoredTrustScore = restoredTrustScore,
                ActionsTaken = actionsTaken,
                Timestamp = DateTime.UtcNow
            };
            _db.TechnicianRedemptionLogs.Add(entry);
            await _db.SaveChangesAsync();
        }
    }
}
