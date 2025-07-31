using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data;
using Data.Models;

namespace Services.Admin
{
    public class TrustGraphAnalyzerService
    {
        private readonly ApplicationDbContext _db;
        public TrustGraphAnalyzerService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<TrustAnomalyLog>> AnalyzeTrustGraphAsync()
        {
            // Stub: Return all anomalies for now
            return await _db.TrustAnomalyLogs.OrderByDescending(x => x.DetectedAt).ToListAsync();
        }

        public async Task<List<TrustAnomalyLog>> DetectAnomaliesAsync(int? technicianId = null)
        {
            var query = _db.TrustAnomalyLogs.AsQueryable();
            if (technicianId.HasValue)
                query = query.Where(x => x.TechnicianId == technicianId.Value);
            return await query.OrderByDescending(x => x.AnomalyScore).ToListAsync();
        }

        public async Task LogAnomalyAsync(int technicianId, string anomalyType, string graphNodeContext, double anomalyScore, string reviewedBy = "", string status = "Unreviewed")
        {
            var log = new TrustAnomalyLog
            {
                TechnicianId = technicianId,
                AnomalyType = anomalyType,
                DetectedAt = DateTime.UtcNow,
                GraphNodeContext = graphNodeContext,
                AnomalyScore = anomalyScore,
                ReviewedBy = reviewedBy,
                Status = status
            };
            _db.TrustAnomalyLogs.Add(log);
            await _db.SaveChangesAsync();
        }
    }
}
