using System;
using MVP_Core.Data.Models;

namespace MVP_Core.Data.Models
{
    public class GpsDriftEventLog
    {
        public int Id { get; set; }
        public int TechnicianId { get; set; }
        public double ScheduledLatitude { get; set; }
        public double ScheduledLongitude { get; set; }
        public double ActualLatitude { get; set; }
        public double ActualLongitude { get; set; }
        public DateTime DriftDetectedAt { get; set; }
        public double DistanceDriftMeters { get; set; }
        public int RouteId { get; set; }
        public string SeverityLevel { get; set; } = string.Empty;
        public string ResolutionStatus { get; set; } = string.Empty;
        public Technician? Technician { get; set; }
    }
}