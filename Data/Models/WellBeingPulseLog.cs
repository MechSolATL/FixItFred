using System;

namespace Data.Models
{
    // Remove duplicate StressLevel enum, now defined in AccountabilityDelayLog.cs
    public class WellBeingPulseLog
    {
        public int Id { get; set; }
        public string EmployeeId { get; set; } = string.Empty; // Sprint 78.1: Initialized to prevent CS8618 warning
        public DateTime Timestamp { get; set; }
        public int MoodScore { get; set; } // 1 to 10
        public StressLevel StressLevel { get; set; } // Enum
        public int WorkSatisfactionScore { get; set; } // 1 to 10
        public string? OpenNote { get; set; }
        public bool NeedsFollowUp { get; set; }
        public bool ManagerReviewed { get; set; }
        public DateTime? ResponseDate { get; set; }
    }
}
