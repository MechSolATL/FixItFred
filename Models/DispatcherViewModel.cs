using System.Collections.Generic;
using MVP_Core.Data.Models;
using MVP_Core.Services.Dispatcher;

namespace MVP_Core.Models
{
    public class DispatcherViewModel
    {
        public List<Technician> Technicians { get; set; } = new();
        public List<ServiceRequest> ServiceRequests { get; set; } = new();
        public List<TechMapPin> TechMapPins { get; set; } = new();
        // Optional: filters
        public string? ProximityFilter { get; set; }
        public string? AvailabilityFilter { get; set; }
        public Dictionary<int, string> TechnicianStatuses { get; set; } = new(); // Sprint 86.7 — TechId -> Status (Available, Lunch, Restocking, Idle)
    }
}
