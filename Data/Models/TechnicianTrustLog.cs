using System;

namespace Data.Models
{
    public class TechnicianTrustLog
    {
        /// <summary>
        /// The unique identifier for the technician trust log entry.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The ID of the technician associated with the trust log entry.
        /// </summary>
        public int TechnicianId { get; set; }

        /// <summary>
        /// The trust score of the technician.
        /// </summary>
        public decimal TrustScore { get; set; }

        /// <summary>
        /// The weight of flags affecting the trust score.
        /// </summary>
        public decimal FlagWeight { get; set; }

        /// <summary>
        /// The timestamp when the trust log entry was recorded.
        /// </summary>
        public DateTime RecordedAt { get; set; }

        /// <summary>
        /// The timestamp when the last reward was sent to the technician, if applicable.
        /// </summary>
        public DateTime? LastRewardSentAt { get; set; }
    }
}
