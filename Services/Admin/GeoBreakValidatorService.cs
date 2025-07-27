using MVP_Core.Data;
using MVP_Core.Data.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace MVP_Core.Services.Admin
{
    public class GeoBreakValidatorService
    {
        private readonly ApplicationDbContext _db;
        public GeoBreakValidatorService(ApplicationDbContext db)
        {
            _db = db;
        }
        // Verifies geo-coordinates of break location
        public GeoBreakValidationLog ValidateBreakLocation(int technicianId, double latitude, double longitude, string jobZone)
        {
            // For demo: flag mismatch if not within 0.05 of jobZone (mock logic)
            bool isMatch = Math.Abs(latitude - 33.75) < 0.05 && Math.Abs(longitude + 84.39) < 0.05;
            var log = new GeoBreakValidationLog
            {
                TechnicianId = technicianId,
                Latitude = latitude,
                Longitude = longitude,
                LocationStatus = isMatch ? "Stationary" : "Moving",
                MinutesStationary = isMatch ? 30 : 0,
                SystemDecision = isMatch ? "Unlock" : "Block",
                ValidationTime = DateTime.UtcNow
            };
            _db.GeoBreakValidationLogs.Add(log);
            _db.SaveChanges();
            return log;
        }
        // Flags mismatch from registered job zone
        public List<GeoBreakValidationLog> GetFlaggedMismatches() => _db.GeoBreakValidationLogs.Where(l => l.SystemDecision == "Block").OrderByDescending(l => l.ValidationTime).ToList();
        // Logs validation result and trigger
        public List<GeoBreakValidationLog> GetAllLogs() => _db.GeoBreakValidationLogs.OrderByDescending(l => l.ValidationTime).ToList();
    }
}
