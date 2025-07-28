using MVP_Core.Data.Models;
using MVP_Core.Data;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using TechnicianModel = MVP_Core.Data.Models.Technician;

namespace MVP_Core.Services.Admin
{
    public class RoutePlaybackService
    {
        private readonly ApplicationDbContext _db;
        public RoutePlaybackService(ApplicationDbContext db)
        {
            _db = db;
        }
        public List<TechnicianModel> GetTechnicians() => _db.Technicians.OrderBy(t => t.FullName).ToList();
        public List<ServiceRequest> GetJobs() => _db.ServiceRequests.OrderByDescending(j => j.CreatedAt).Take(100).ToList();
        public List<TechnicianAuditLog> GetTrail(int technicianId, string? routeTag = null)
        {
            var query = _db.TechnicianAuditLogs.Where(l => l.TechnicianId == technicianId && l.Source == "GPS" && l.Latitude != null && l.Longitude != null);
            if (!string.IsNullOrEmpty(routeTag))
                query = query.Where(l => l.RouteTag == routeTag);
            return query.OrderBy(l => l.Timestamp).ToList();
        }
        public string GetTrailGeoJson(List<TechnicianAuditLog> logs)
        {
            var points = logs.Select(l => new[] { l.Longitude ?? 0, l.Latitude ?? 0 }).ToList();
            var geoJson = new
            {
                type = "Feature",
                geometry = new
                {
                    type = "LineString",
                    coordinates = points
                },
                properties = new { }
            };
            return JsonConvert.SerializeObject(geoJson);
        }
        public string GetTrailPointsJson(List<TechnicianAuditLog> logs)
        {
            return JsonConvert.SerializeObject(logs.Select(l => new {
                lat = l.Latitude,
                lng = l.Longitude,
                timestamp = l.Timestamp,
                action = l.ActionType.ToString(),
                notes = l.Notes
            }));
        }
    }
}
