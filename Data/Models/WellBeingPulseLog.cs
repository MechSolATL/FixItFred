using System;

namespace MVP_Core.Data.Models
{
    public enum StressLevel
    {
        Low = 1,
        Moderate = 2,
        High = 3,
        Severe = 4
    }

    public class WellBeingPulseLog
    {
        public int Id { get; set; }
        public string EmployeeId { get; set; }
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
