using System;
using System.ComponentModel.DataAnnotations;

namespace MVP_Core.Models
{
    // Sprint 86.2 — Technician Accountability Engine
    public class TechnicianBehaviorLog
    {
        [Key]
        public int Id { get; set; }
        public int TechnicianId { get; set; }
        public int ServiceRequestId { get; set; }
        [Required]
        public string ViolationType { get; set; } = string.Empty; // e.g. Time, Location, Metadata
        public string Notes { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        [Required]
        public string Status { get; set; } = string.Empty; // Flagged, Cleared, Escalated

        // --- UI compatibility for TechAudit ---
        public string ActionType { get; set; } = string.Empty; // For UI filter compatibility
        public double? Latitude { get; set; } // For location display (optional)
        public double? Longitude { get; set; } // For location display (optional)
        public string Source { get; set; } = string.Empty; // For source display (optional)
        public string TriggerType { get; set; } = string.Empty; // Sprint 86.7 — e.g. "AutoLoggedArrival", "LateETAAlertSent"
        public bool Acknowledged { get; set; } = false; // Sprint 86.7 — Whether tech acknowledged prompt
    }
}
