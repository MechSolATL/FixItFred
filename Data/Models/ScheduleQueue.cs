// FixItFred: Created for Scheduler Queue module under Nova’s directive
// Sprint 30A – 2025-07-25
// Purpose: Technician scheduling queue for dispatch and ETA routing
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVP_Core.Data.Models
{
    public enum ScheduleStatus
    {
        Pending,
        Dispatched,
        Cancelled
    }

    public class ScheduleQueue
    {
        public int Id { get; set; }
        public int TechnicianId { get; set; }
        public DateTime ScheduledTime { get; set; }
        public string Zone { get; set; } = string.Empty;
        public ScheduleStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        // FK
        public Technician? Technician { get; set; }

        // FixItFred: Sprint 30A compatibility patch — added view-bound properties
        public string AssignedTechnicianName { get; set; } = string.Empty;
        public string TechnicianStatus { get; set; } = string.Empty;
        public DateTime? EstimatedArrival { get; set; }
        public int ServiceRequestId { get; set; }
        public DateTime? ScheduledFor { get; set; }
        // FixItFred: Sprint 34.1 — SLA Escalation
        public DateTime? SLAExpiresAt { get; set; }
        // Sprint 50.0: Intelligence fields
        public DateTime? SLAWindowStart { get; set; }
        public DateTime? SLAWindowEnd { get; set; }
        public double? GeoDistanceKm { get; set; } // Distance to job in km
        public bool IsTechnicianAvailable { get; set; } // Real-time availability
        public string ServiceTypePriority { get; set; } = string.Empty; // e.g. Emergency, Urgent, Routine
        public bool IsUrgent { get; set; } // DB flag for urgent
        public bool IsEmergency { get; set; } // DB flag for emergency
        public bool DispatcherOverride { get; set; } // True if manually overridden
        public string? OverrideReason { get; set; } // Reason for override
        public double? EstimatedDurationHours { get; set; }
        public decimal? CommissionAmount { get; set; }
        // Sprint 54.0: Smart Dispatch fields
        public double? GeoDistanceToJob { get; set; } // Mapbox-calculated distance (km)
        public TimeSpan? OptimizedETA { get; set; } // Mapbox-calculated ETA
        public double? RouteScore { get; set; } // Composite score for dispatch optimization
        public int? PreferredTechnicianId { get; set; } // ID of top suggested tech
    }

    public class ScheduleHistory
    {
        public int Id { get; set; }
        public int TechnicianId { get; set; }
        public DateTime ScheduledTime { get; set; }
        public string Zone { get; set; } = string.Empty;
        public ScheduleStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        // FK
        public Technician? Technician { get; set; }
    }

    public class NotificationsSent
    {
        public int Id { get; set; }
        public int TechnicianId { get; set; }
        public DateTime ScheduledTime { get; set; }
        public string Zone { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        // FK
        public Technician? Technician { get; set; }
    }
}
