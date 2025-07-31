using System;

namespace Data.Models
{
    public class TechnicianInsightLog
    {
        /// <summary>
        /// The unique identifier for the technician insight log entry.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The ID of the technician associated with the insight log entry.
        /// </summary>
        public int TechnicianId { get; set; }

        /// <summary>
        /// The type of insight logged (e.g., performance, feedback).
        /// </summary>
        public string InsightType { get; set; } = string.Empty;

        /// <summary>
        /// The detailed description of the insight.
        /// </summary>
        public string InsightDetail { get; set; } = string.Empty;

        /// <summary>
        /// The timestamp when the insight was logged.
        /// </summary>
        public DateTime LoggedAt { get; set; }

        /// <summary>
        /// The score associated with the insight, used for forecast calculations.
        /// </summary>
        public decimal Score { get; set; } // Used for forecast calculations
    }
}
