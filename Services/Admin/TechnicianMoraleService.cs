using MVP_Core.Data.Models;
using MVP_Core.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace MVP_Core.Services.Admin
{
    public class TechnicianMoraleService
    {
        private readonly ApplicationDbContext _db;
        public TechnicianMoraleService(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<List<TechnicianMoraleLog>> GetMoraleLogsAsync(int technicianId)
        {
            return await _db.TechnicianMoraleLogs
                .Where(m => m.TechnicianId == technicianId)
                .OrderByDescending(m => m.Timestamp)
                .ToListAsync();
        }
        public async Task AddMoraleEntryAsync(int technicianId, int moraleScore, string notes, int trustImpact)
        {
            var entry = new TechnicianMoraleLog
            {
                TechnicianId = technicianId,
                MoraleScore = moraleScore,
                Notes = notes,
                Timestamp = DateTime.UtcNow,
                TrustImpact = trustImpact
            };
            _db.TechnicianMoraleLogs.Add(entry);
            await _db.SaveChangesAsync();
        }
    }
}
