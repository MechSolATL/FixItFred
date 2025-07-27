using MVP_Core.Data;
using MVP_Core.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVP_Core.Services.Admin
{
    /// <summary>
    /// Builds directional weighted reputation graphs and detects clusters of correlated influence.
    /// </summary>
    public class ReputationGraphBuilderService
    {
        private readonly ApplicationDbContext _db;
        public ReputationGraphBuilderService(ApplicationDbContext db)
        {
            _db = db;
        }
        // Build reputation graph edges for all technicians
        public async Task<List<TechnicianReputationEdge>> BuildGraphAsync()
        {
            // Example: fetch all edges
            return await _db.TechnicianReputationEdges.AsNoTracking().ToListAsync();
        }
        // Detect clusters of positive/negative influence
        public async Task<List<List<int>>> DetectInfluenceClustersAsync(double threshold = 0.0)
        {
            var edges = await _db.TechnicianReputationEdges.AsNoTracking().ToListAsync();
            // Simple stub: group by positive/negative trust
            var positive = edges.Where(e => e.TrustWeight > threshold).Select(e => e.TargetTechnicianId).Distinct().ToList();
            var negative = edges.Where(e => e.TrustWeight < -threshold).Select(e => e.TargetTechnicianId).Distinct().ToList();
            return new List<List<int>> { positive, negative };
        }
    }

    /// <summary>
    /// Fuses dispute logs into the reputation graph and tags dispute sources.
    /// </summary>
    public class DisputeFusionAnalyzerService
    {
        private readonly ApplicationDbContext _db;
        public DisputeFusionAnalyzerService(ApplicationDbContext db)
        {
            _db = db;
        }
        // Fuse dispute logs into graph
        public async Task<List<DisputeFusionLog>> FuseDisputesAsync()
        {
            return await _db.DisputeFusionLogs.AsNoTracking().OrderByDescending(x => x.LoggedAt).ToListAsync();
        }
        // Tag dispute sources
        public async Task<List<string>> GetSourceTagsAsync()
        {
            return await _db.DisputeFusionLogs.Select(x => x.SourceTag).Distinct().ToListAsync();
        }
    }
}
