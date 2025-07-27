using MVP_Core.Data;
using MVP_Core.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVP_Core.Services.Admin
{
    public class TechnicianConflictRadarService
    {
        private readonly ApplicationDbContext _db;
        public TechnicianConflictRadarService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task LogConflictAsync(int technicianId, int conflictWithId, string conflictType, int severityLevel, double trustImpactScore, string resolutionNotes = "")
        {
            var log = new TechnicianConflictLog
            {
                TechnicianId = technicianId,
                ConflictWithId = conflictWithId,
                ConflictType = conflictType,
                OccurredAt = DateTime.UtcNow,
                SeverityLevel = severityLevel,
                TrustImpactScore = trustImpactScore,
                ResolutionNotes = resolutionNotes,
                Resolved = false
            };
            _db.TechnicianConflictLogs.Add(log);
            await _db.SaveChangesAsync();
        }

        public async Task<List<TechnicianConflictLog>> GetActiveConflictsAsync(int? technicianId = null)
        {
            var query = _db.TechnicianConflictLogs.Where(x => !x.Resolved);
            if (technicianId.HasValue)
                query = query.Where(x => x.TechnicianId == technicianId.Value || x.ConflictWithId == technicianId.Value);
            return await query.OrderByDescending(x => x.OccurredAt).ToListAsync();
        }

        public async Task<bool> ResolveConflictAsync(int conflictId, string resolutionNotes)
        {
            var conflict = await _db.TechnicianConflictLogs.FindAsync(conflictId);
            if (conflict == null) return false;
            conflict.Resolved = true;
            conflict.ResolutionNotes = resolutionNotes;
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
