using System;

namespace Data.Models
{
    public class TechnicianSessionTelemetry
    {
        public int Id { get; set; }
        public int TechnicianId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int TotalDurationMinutes { get; set; }
        public int Interruptions { get; set; }
        public string DeviceType { get; set; } = string.Empty;
        public string SessionId { get; set; } = string.Empty;
        public double GpsAccuracyAvg { get; set; }
        public int SignalStrength { get; set; }
        public string UptimeStatus { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public DateTime LoggedAt { get; set; }
        public Technician? Technician { get; set; }
    }
}