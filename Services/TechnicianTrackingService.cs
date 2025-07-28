using MVP_Core.Data;
using MVP_Core.Data.Models;
using MVP_Core.Hubs;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVP_Core.Services
{
    // Sprint 91.7: TechnicianTrackingService for live location and ETA logic
    public class TechnicianTrackingService
    {
        private readonly ApplicationDbContext _db;
        private readonly IHubContext<TechnicianTrackingHub> _hubContext; // Sprint 91.7.Part4
        public TechnicianTrackingService(ApplicationDbContext db, IHubContext<TechnicianTrackingHub> hubContext)
        {
            _db = db;
            _hubContext = hubContext;
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

        // Sprint 91.7: Return all techs with latest location, status, and ghost trail
        public List<TechnicianTrackingDto> GetAllTechnicianLocations()
        {
            var techs = _db.Technicians.ToList();
            var logs = _db.TechTrackingLogs.ToList();
            var result = new List<TechnicianTrackingDto>();
            foreach (var tech in techs)
            {
                var lastLogs = logs.Where(l => l.TechnicianId == tech.Id)
                                   .OrderByDescending(l => l.Timestamp)
                                   .Take(5)
                                   .OrderBy(l => l.Timestamp)
                                   .ToList();
                var latest = lastLogs.LastOrDefault();
                result.Add(new TechnicianTrackingDto
                {
                    TechnicianId = tech.Id,
                    Name = tech.FullName,
                    TruckId = $"TRK-{tech.Id:D3}", // Sprint 91.7: Mock truck ID
                    ServiceType = tech.Specialty ?? "General",
                    Status = MockStatus(tech),
                    Latitude = latest?.Latitude ?? tech.Latitude ?? 33.75,
                    Longitude = latest?.Longitude ?? tech.Longitude ?? -84.39,
                    GhostTrail = lastLogs.Select(l => new TechnicianGhostTrailPoint
                    {
                        Latitude = l.Latitude,
                        Longitude = l.Longitude,
                        Timestamp = l.Timestamp
                    }).ToList(),
                    ETA = MockEta(),
                });
            }
            return result;
        }

        // Sprint 91.7.Part4: Broadcast live updates to all SignalR clients
        public async Task BroadcastLiveUpdatesAsync()
        {
            var techs = GetAllTechnicianLocations();
            await _hubContext.Clients.All.SendAsync("ReceiveLocationUpdate", techs);
        }

        // Sprint 91.7: Mock status logic
        private string MockStatus(MVP_Core.Data.Models.Technician tech)
        {
            var statuses = new[] { "En Route", "Working", "Idle", "Delayed" };
            return statuses[tech.Id % statuses.Length];
        }

        // Sprint 91.7: Mock ETA logic
        private string MockEta()
        {
            var rnd = new Random();
            return $"{rnd.Next(5, 45)} min";
        }
    }

    // Sprint 91.7: DTO for map overlay
    public class TechnicianTrackingDto
    {
        public int TechnicianId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string TruckId { get; set; } = string.Empty;
        public string ServiceType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public List<TechnicianGhostTrailPoint> GhostTrail { get; set; } = new();
        public string ETA { get; set; } = string.Empty;
    }
    public class TechnicianGhostTrailPoint
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
