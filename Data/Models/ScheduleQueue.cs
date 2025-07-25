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
