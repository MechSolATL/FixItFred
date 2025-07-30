using System;

namespace MVP_Core.Data.Models
{
    public enum StressLevel
    {
        /// <summary>
        /// Stress level 1.
        /// </summary>
        Level1 = 1,
        /// <summary>
        /// Stress level 2.
        /// </summary>
        Level2 = 2,
        /// <summary>
        /// Stress level 3.
        /// </summary>
        Level3 = 3,
        /// <summary>
        /// Stress level 4.
        /// </summary>
        Level4 = 4,
        /// <summary>
        /// Stress level 5.
        /// </summary>
        Level5 = 5,
        /// <summary>
        /// Stress level 6.
        /// </summary>
        Level6 = 6,
        /// <summary>
        /// Stress level 7.
        /// </summary>
        Level7 = 7,
        /// <summary>
        /// Stress level 8.
        /// </summary>
        Level8 = 8,
        /// <summary>
        /// Stress level 9.
        /// </summary>
        Level9 = 9,
        /// <summary>
        /// Stress level 10.
        /// </summary>
        Level10 = 10
    }

    public class AccountabilityDelayLog
    {
        /// <summary>
        /// The unique identifier for the accountability delay log.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The ID of the employee associated with the delay.
        /// </summary>
        public string EmployeeId { get; set; } = string.Empty;

        /// <summary>
        /// The timestamp when the delay log was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// The timestamp when the delay was resolved.
        /// </summary>
        public DateTime? ResolutionTimestamp { get; set; }

        /// <summary>
        /// Comments provided by the reviewer.
        /// </summary>
        public string? ReviewerComment { get; set; }

        /// <summary>
        /// Indicates whether the delay was escalated.
        /// </summary>
        public bool IsEscalated { get; set; }

        /// <summary>
        /// Indicates whether the delay was flagged.
        /// </summary>
        public bool IsFlagged { get; set; }

        /// <summary>
        /// The duration of the delay in hours.
        /// </summary>
        public int DelayDurationHours { get; set; }

        /// <summary>
        /// The ID of the party responsible for the delay.
        /// </summary>
        public string? ResponsiblePartyId { get; set; }

        /// <summary>
        /// The metric used to measure accountability.
        /// </summary>
        public string? AccountabilityMetric { get; set; }

        /// <summary>
        /// Additional notes regarding the delay.
        /// </summary>
        public string? Notes { get; set; }

        /// <summary>
        /// The timestamp when the log was last updated.
        /// </summary>
        public DateTime? LastUpdated { get; set; }
    }
}
