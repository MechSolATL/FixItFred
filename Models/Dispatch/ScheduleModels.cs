// FixItFred Patch Log — Sprint 27 SaaS Modularization
// [2024-07-25T00:00:00Z] — Added RegionId for multi-region/tenant support.
// FixItFred Patch Log — Sprint 26.4
// [2025-07-25T00:00:00Z] — Scheduler/dispatch models scaffolded for queue, history, and notifications.
using System;
using System.Collections.Generic;

namespace MVP_Core.Models.Dispatch
{
    public class ScheduleQueue
    {
        public int Id { get; set; }
        public int TechnicianId { get; set; }
        public int ServiceRequestId { get; set; }
        public DateTime ScheduledFor { get; set; }
        public string Status { get; set; } = "Pending";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        // Sprint 26.4B enhancements:
        public string? AssignedTechnicianName { get; set; }
        public string? TechnicianStatus { get; set; }
        public DateTime? EstimatedArrival { get; set; }
        public string? ServiceZone { get; set; } // For travel time stub
        public string? ZipCode { get; set; } // For travel time stub
        public string? RegionId { get; set; } // SaaS/white-label region/tenant context
    }

    public class ScheduleHistory
    {
        public int Id { get; set; }
        public int ScheduleQueueId { get; set; }
        public string Action { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string? Notes { get; set; }
        public string? RegionId { get; set; } // SaaS/white-label region/tenant context
    }

    public class NotificationsSent
    {
        public int Id { get; set; }
        public int TechnicianId { get; set; }
        public int ServiceRequestId { get; set; }
        public string NotificationType { get; set; } = string.Empty;
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public string? Message { get; set; }
        public string? RegionId { get; set; } // SaaS/white-label region/tenant context
    }
}
