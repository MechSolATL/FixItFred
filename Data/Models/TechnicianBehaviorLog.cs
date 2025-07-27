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
        public string BehaviorType { get; set; } = string.Empty; // Sprint 78.1: Initialized to prevent CS8618 warning
        public string Description { get; set; } = string.Empty; // Sprint 78.1: Initialized to prevent CS8618 warning
        public string SeverityLevel { get; set; } = string.Empty; // Sprint 78.1: Initialized to prevent CS8618 warning
        public DateTime Timestamp { get; set; }
    }
}
