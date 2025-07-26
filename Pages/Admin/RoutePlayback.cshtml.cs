using MVP_Core.Data;
using MVP_Core.Data.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace MVP_Core.Pages.Admin
{
    public class RoutePlaybackModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public RoutePlaybackModel(ApplicationDbContext db)
        {
            _db = db;
        }

        [BindProperty(SupportsGet = true)]
        public int? TechnicianId { get; set; }
        [BindProperty(SupportsGet = true)]
        public int? ServiceRequestId { get; set; }
        public List<MVP_Core.Data.Models.Technician> Technicians { get; set; } = new();
        public List<ServiceRequest> Jobs { get; set; } = new();
        public string TrailGeoJson { get; set; } = "";
        public string TrailPointsJson { get; set; } = "";
        public List<TechnicianAuditLog> TrailLogs { get; set; } = new();

        public async Task OnGetAsync()
        {
            Technicians = _db.Technicians.OrderBy(t => t.FullName).ToList();
            Jobs = _db.ServiceRequests.OrderByDescending(j => j.CreatedAt).Take(100).ToList();
            if (TechnicianId.HasValue && ServiceRequestId.HasValue)
            {
                TrailLogs = _db.TechnicianAuditLogs
                    .Where(l => l.TechnicianId == TechnicianId.Value && l.Source == "GPS" && l.Latitude != null && l.Longitude != null)
                    .OrderBy(l => l.Timestamp)
                    .ToList();
                // Optionally filter by RouteTag or ServiceRequestId if available
                // Compose GeoJSON LineString
                var points = TrailLogs.Select(l => new[] { l.Longitude ?? 0, l.Latitude ?? 0 }).ToList();
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
                TrailGeoJson = JsonConvert.SerializeObject(geoJson);
                TrailPointsJson = JsonConvert.SerializeObject(TrailLogs.Select(l => new {
                    lat = l.Latitude,
                    lng = l.Longitude,
                    timestamp = l.Timestamp,
                    action = l.ActionType.ToString(),
                    notes = l.Notes
                }));
            }
            await Task.CompletedTask;
        }
    }
}
