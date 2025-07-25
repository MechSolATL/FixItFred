// FixItFred: Sprint 30A - Represents logged ETA changes for technicians/zones
// Sprint 30A patch — added tracking fields used in Razor view
using System;

namespace MVP_Core.Data.Models
{
    public class ETAHistoryEntry
    {
        public int Id { get; set; }
        public int TechnicianId { get; set; }
        public string Zone { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }

        // FixItFred: Sprint 30A patch — added tracking fields used in Razor view
        public string TechnicianName { get; set; } = string.Empty;
        public int ServiceRequestId { get; set; }
        public DateTime? PredictedETA { get; set; }
        public DateTime? ActualArrival { get; set; }
    }
}
