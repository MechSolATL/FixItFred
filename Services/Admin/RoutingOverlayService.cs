using MVP_Core.Data;
using MVP_Core.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace MVP_Core.Services.Admin
{
    public class RoutingOverlayService
    {
        private readonly ApplicationDbContext _db;
        public RoutingOverlayService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<RoutingOverlayRegion> CreateOverlay(string regionName, string geoBoundaryJson, string preferredTechIds, int zonePriority)
        {
            var region = new RoutingOverlayRegion
            {
                RegionName = regionName,
                GeoBoundaryJson = geoBoundaryJson,
                PreferredTechIds = preferredTechIds,
                ZonePriority = zonePriority,
                CreatedAt = DateTime.UtcNow
            };
            _db.RoutingOverlayRegions.Add(region);
            await _db.SaveChangesAsync();
            return region;
        }

        public async Task UpdateRegionPriority(int regionId, int newPriority)
        {
            var region = await _db.RoutingOverlayRegions.FindAsync(regionId);
            if (region != null)
            {
                region.ZonePriority = newPriority;
                await _db.SaveChangesAsync();
            }
        }

        public async Task AssignTechnicianRouting(int regionId, string preferredTechIds)
        {
            var region = await _db.RoutingOverlayRegions.FindAsync(regionId);
            if (region != null)
            {
                region.PreferredTechIds = preferredTechIds;
                await _db.SaveChangesAsync();
            }
        }
    }
}
