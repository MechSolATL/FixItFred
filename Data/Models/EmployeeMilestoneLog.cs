namespace Data.Models
{
    public class EmployeeMilestoneLog
    {
        /// <summary>
        /// The unique identifier for the employee milestone log.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The ID of the employee associated with the milestone.
        /// </summary>
        public string EmployeeId { get; set; } = string.Empty;

        /// <summary>
        /// The type of milestone (e.g., "Birthday" or "Anniversary").
        /// </summary>
        public string MilestoneType { get; set; } = string.Empty;

        /// <summary>
        /// The date when the milestone was recognized.
        /// </summary>
        public DateTime DateRecognized { get; set; }

        /// <summary>
        /// Indicates whether the milestone was broadcasted.
        /// </summary>
        public bool Broadcasted { get; set; }

        /// <summary>
        /// A custom message associated with the milestone.
        /// </summary>
        public string CustomMessage { get; set; } = string.Empty;

        /// <summary>
        /// Indicates whether the employee has opted out of roasts.
        /// </summary>
        public bool IsOptedOutOfRoasts { get; set; } = false;

        /// <summary>
        /// The timestamp when the last roast was delivered.
        /// </summary>
        public DateTime? LastRoastDeliveredAt { get; set; }

        /// <summary>
        /// The preferred roast tier for the employee, if applicable.
        /// </summary>
        public string? RoastTierPreference { get; set; }
    }
}
