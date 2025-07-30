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
        /// <summary>
        /// Indicates that the schedule is pending.
        /// </summary>
        Pending,

        /// <summary>
        /// Indicates that the schedule has been dispatched.
        /// </summary>
        Dispatched,

        /// <summary>
        /// Indicates that the schedule has been cancelled.
        /// </summary>
        Cancelled
    }

    public class ScheduleQueue
    {
        /// <summary>
        /// The unique identifier for the schedule queue entry.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The ID of the technician assigned to the schedule.
        /// </summary>
        public int TechnicianId { get; set; }

        /// <summary>
        /// The scheduled time for the technician.
        /// </summary>
        public DateTime ScheduledTime { get; set; }

        /// <summary>
        /// The zone associated with the schedule.
        /// </summary>
        public string Zone { get; set; } = string.Empty;

        /// <summary>
        /// The status of the schedule.
        /// </summary>
        public ScheduleStatus Status { get; set; }

        /// <summary>
        /// The timestamp when the schedule was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// The technician associated with the schedule.
        /// </summary>
        public Technician? Technician { get; set; }

        /// <summary>
        /// The name of the technician to whom the schedule is assigned.
        /// </summary>
        public string AssignedTechnicianName { get; set; } = string.Empty;

        /// <summary>
        /// The status of the technician for this schedule.
        /// </summary>
        public string TechnicianStatus { get; set; } = string.Empty;

        /// <summary>
        /// The estimated arrival time of the technician.
        /// </summary>
        public DateTime? EstimatedArrival { get; set; }

        /// <summary>
        /// The ID of the service request associated with this schedule.
        /// </summary>
        public int ServiceRequestId { get; set; }

        /// <summary>
        /// The time for which the service is scheduled.
        /// </summary>
        public DateTime? ScheduledFor { get; set; }

        /// <summary>
        /// The expiration time for the SLA associated with this schedule.
        /// </summary>
        public DateTime? SLAExpiresAt { get; set; }

        /// <summary>
        /// The start time of the SLA window for this schedule.
        /// </summary>
        public DateTime? SLAWindowStart { get; set; }

        /// <summary>
        /// The end time of the SLA window for this schedule.
        /// </summary>
        public DateTime? SLAWindowEnd { get; set; }

        /// <summary>
        /// The geographical distance to the job location in kilometers.
        /// </summary>
        public double? GeoDistanceKm { get; set; }

        /// <summary>
        /// Indicates real-time availability of the technician.
        /// </summary>
        public bool IsTechnicianAvailable { get; set; }

        /// <summary>
        /// The priority of the service type for this schedule.
        /// </summary>
        public string ServiceTypePriority { get; set; } = string.Empty;

        /// <summary>
        /// Indicates if the schedule is marked as urgent.
        /// </summary>
        public bool IsUrgent { get; set; }

        /// <summary>
        /// Indicates if the schedule is marked as emergency.
        /// </summary>
        public bool IsEmergency { get; set; }

        /// <summary>
        /// Indicates if the schedule has been manually overridden by a dispatcher.
        /// </summary>
        public bool DispatcherOverride { get; set; }

        /// <summary>
        /// The reason for the manual override of the schedule.
        /// </summary>
        public string? OverrideReason { get; set; }

        /// <summary>
        /// The estimated duration of the job in hours.
        /// </summary>
        public double? EstimatedDurationHours { get; set; }

        /// <summary>
        /// The commission amount associated with the schedule.
        /// </summary>
        public decimal? CommissionAmount { get; set; }

        /// <summary>
        /// The distance to the job calculated by Mapbox in kilometers.
        /// </summary>
        public double? GeoDistanceToJob { get; set; }

        /// <summary>
        /// The optimized ETA calculated by Mapbox.
        /// </summary>
        public TimeSpan? OptimizedETA { get; set; }

        /// <summary>
        /// The composite score for dispatch optimization.
        /// </summary>
        public double? RouteScore { get; set; }

        /// <summary>
        /// The ID of the preferred technician suggested by the system.
        /// </summary>
        public int? PreferredTechnicianId { get; set; }

        /// <summary>
        /// Indicates if the job has been auto-escalated due to SLA collision.
        /// </summary>
        public bool IsEscalated { get; set; }
    }

    public class ScheduleHistory
    {
        /// <summary>
        /// The unique identifier for the schedule history entry.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The ID of the technician associated with the schedule history.
        /// </summary>
        public int TechnicianId { get; set; }

        /// <summary>
        /// The scheduled time for the technician.
        /// </summary>
        public DateTime ScheduledTime { get; set; }

        /// <summary>
        /// The zone associated with the schedule history.
        /// </summary>
        public string Zone { get; set; } = string.Empty;

        /// <summary>
        /// The status of the schedule history.
        /// </summary>
        public ScheduleStatus Status { get; set; }

        /// <summary>
        /// The timestamp when the schedule history was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// The technician associated with the schedule history.
        /// </summary>
        public Technician? Technician { get; set; }
    }

    public class NotificationsSent
    {
        /// <summary>
        /// The unique identifier for the notification sent entry.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The ID of the technician associated with the notification.
        /// </summary>
        public int TechnicianId { get; set; }

        /// <summary>
        /// The scheduled time for the notification.
        /// </summary>
        public DateTime ScheduledTime { get; set; }

        /// <summary>
        /// The zone associated with the notification.
        /// </summary>
        public string Zone { get; set; } = string.Empty;

        /// <summary>
        /// The status of the notification.
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// The timestamp when the notification was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// The technician associated with the notification.
        /// </summary>
        public Technician? Technician { get; set; }
    }
}
