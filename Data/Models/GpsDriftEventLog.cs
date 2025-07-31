using System;

namespace Data.Models
{
    public class GpsDriftEventLog
    {
        /// <summary>
        /// The unique identifier for the GPS drift event log.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The ID of the technician associated with the GPS drift event.
        /// </summary>
        public int TechnicianId { get; set; }

        /// <summary>
        /// The scheduled latitude of the technician's location.
        /// </summary>
        public double ScheduledLatitude { get; set; }

        /// <summary>
        /// The scheduled longitude of the technician's location.
        /// </summary>
        public double ScheduledLongitude { get; set; }

        /// <summary>
        /// The actual latitude of the technician's location.
        /// </summary>
        public double ActualLatitude { get; set; }

        /// <summary>
        /// The actual longitude of the technician's location.
        /// </summary>
        public double ActualLongitude { get; set; }

        /// <summary>
        /// The timestamp when the GPS drift was detected.
        /// </summary>
        public DateTime DriftDetectedAt { get; set; }

        /// <summary>
        /// The distance of the GPS drift in meters.
        /// </summary>
        public double DistanceDriftMeters { get; set; }

        /// <summary>
        /// The ID of the associated route.
        /// </summary>
        public int RouteId { get; set; }

        /// <summary>
        /// The severity level of the GPS drift event.
        /// </summary>
        public string SeverityLevel { get; set; } = string.Empty;

        /// <summary>
        /// The resolution status of the GPS drift event.
        /// </summary>
        public string ResolutionStatus { get; set; } = string.Empty;

        /// <summary>
        /// The technician associated with the GPS drift event.
        /// </summary>
        public Technician? Technician { get; set; }
    }
}