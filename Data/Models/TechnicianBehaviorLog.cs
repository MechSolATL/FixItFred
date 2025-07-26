using System;

namespace MVP_Core.Data.Models
{
    /// <summary>
    /// Logs technician behavioral anomalies for auto-flagging and analytics.
    /// </summary>
    public class TechnicianBehaviorLog
    {
        public int Id { get; set; }
        public int TechnicianId { get; set; }
        public string BehaviorType { get; set; }  // e.g. "LateStart", "RapidJobSwitch", etc.
        public string Description { get; set; }
        public string SeverityLevel { get; set; } // Info, Warning, Critical
        public DateTime Timestamp { get; set; }
    }
}
