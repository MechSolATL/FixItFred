using System;

namespace MVP_Core.Data.Models
{
    public enum StressLevel
    {
        Level1 = 1,
        Level2 = 2,
        Level3 = 3,
        Level4 = 4,
        Level5 = 5,
        Level6 = 6,
        Level7 = 7,
        Level8 = 8,
        Level9 = 9,
        Level10 = 10
    }

    public class AccountabilityDelayLog
    {
        public int Id { get; set; }
        public string EmployeeId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? ResolutionTimestamp { get; set; }
        public string? ReviewerComment { get; set; }
        public bool IsEscalated { get; set; }
        public bool IsFlagged { get; set; }
        public int DelayDurationHours { get; set; }
        public string? ResponsiblePartyId { get; set; }
        public string? AccountabilityMetric { get; set; }
        public string? Notes { get; set; }
        public DateTime? LastUpdated { get; set; }
    }
}
