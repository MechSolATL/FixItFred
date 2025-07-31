using System;

namespace Data.Models
{
    public class UptimeHeartbeatLog
    {
        public int Id { get; set; }
        public int TechnicianId { get; set; }
        public DateTime HeartbeatAt { get; set; }
        public bool IsActive { get; set; }
        public int BatteryLevel { get; set; }
        public string NetworkType { get; set; } = string.Empty;
        public string SessionId { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public double GeoConfidence { get; set; }
        public string LogSource { get; set; } = string.Empty;
        public Technician? Technician { get; set; }
    }
}