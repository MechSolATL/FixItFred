using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using Data;
using Data.Models;

namespace Services.Admin
{
    public class TechnicianWarningService
    {
        private readonly ApplicationDbContext _db;
        public TechnicianWarningService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<TechnicianWarningLog>> GetWarningsAsync(int? technicianId = null, DateTime? from = null, DateTime? to = null, string? zone = null, string? severity = null)
        {
            var query = _db.TechnicianWarningLogs.AsQueryable();
            if (technicianId.HasValue)
                query = query.Where(x => x.TechnicianId == technicianId.Value);
            if (from.HasValue)
                query = query.Where(x => x.Timestamp >= from.Value);
            if (to.HasValue)
                query = query.Where(x => x.Timestamp <= to.Value);
            if (!string.IsNullOrEmpty(zone))
                query = query.Where(x => x.SourceZone == zone);
            if (!string.IsNullOrEmpty(severity))
                query = query.Where(x => x.Severity == severity);
            return await query.OrderByDescending(x => x.Timestamp).ToListAsync();
        }

        public async Task AddWarningAsync(TechnicianWarningLog log)
        {
            log.Timestamp = DateTime.UtcNow;
            _db.TechnicianWarningLogs.Add(log);
            await _db.SaveChangesAsync();
        }

        public async Task ResolveWarningAsync(int id)
        {
            var warning = await _db.TechnicianWarningLogs.FindAsync(id);
            if (warning != null)
            {
                warning.ResolvedFlag = true;
                await _db.SaveChangesAsync();
            }
        }
    }
}
