using MVP_Core.Data.Models;
using MVP_Core.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace MVP_Core.Services.Admin
{
    public class TrustRedemptionService
    {
        private readonly ApplicationDbContext _db;
        public TrustRedemptionService(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task LogOpportunityAsync(int technicianId, string opportunityType, string? description, int pointsRequired)
        {
            var opp = new RedemptionOpportunity
            {
                TechnicianId = technicianId,
                OpportunityType = opportunityType,
                Description = description,
                PointsRequired = pointsRequired,
                CreatedAt = DateTime.UtcNow,
                Status = "Open"
            };
            _db.RedemptionOpportunities.Add(opp);
            await _db.SaveChangesAsync();
        }
        public async Task ResolveOpportunityAsync(int opportunityId, string resolutionNotes)
        {
            var opp = await _db.RedemptionOpportunities.FindAsync(opportunityId);
            if (opp != null && opp.Status == "Open")
            {
                opp.Status = "Resolved";
                opp.ResolvedAt = DateTime.UtcNow;
                opp.ResolutionNotes = resolutionNotes;
                await _db.SaveChangesAsync();
            }
        }
        public async Task<List<RedemptionOpportunity>> GetOpenRedemptions(int? technicianId = null)
        {
            var query = _db.RedemptionOpportunities.Where(o => o.Status == "Open");
            if (technicianId.HasValue)
                query = query.Where(o => o.TechnicianId == technicianId.Value);
            return await query.OrderByDescending(o => o.CreatedAt).ToListAsync();
        }
    }
}
