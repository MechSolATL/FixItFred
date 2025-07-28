using MVP_Core.Data;
using MVP_Core.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MVP_Core.Services
{
    // Sprint 91.7: TechnicianTrackingService for live location and ETA logic
    public class TechnicianTrackingService
    {
        private readonly ApplicationDbContext _db;
        public TechnicianTrackingService(ApplicationDbContext db)
        {
            _db = db;
        }

        public List<TechTrackingLog> GetRecentLocations(int technicianId, int count = 5)
        {
            // Return last N locations for ghost trail
            return _db.TechTrackingLogs
                .Where(l => l.TechnicianId == technicianId)
                .OrderByDescending(l => l.Timestamp)
                .Take(count)
                .ToList();
        }

        // Sprint 91.7: Add ETA calculation logic here as needed
    }
}
