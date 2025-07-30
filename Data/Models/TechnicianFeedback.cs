using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVP_Core.Data.Models
{
    /// <summary>
    /// TechnicianFeedback: Stores ratings (1-5) and optional notes for technician performance on a specific job.
    /// Notes are optional and can include issue flags or praise. Rating is 1 (poor) to 5 (excellent).
    /// </summary>
    public class TechnicianFeedback
    {
        /// <summary>
        /// The unique identifier for the feedback entry.
        /// </summary>
        [Key]
        public int FeedbackId { get; set; }

        /// <summary>
        /// The ID of the technician associated with the feedback.
        /// </summary>
        public int TechnicianId { get; set; }

        /// <summary>
        /// The ID of the service request associated with the feedback.
        /// </summary>
        public int RequestId { get; set; }

        /// <summary>
        /// The rating given to the technician (1=poor, 5=excellent).
        /// </summary>
        [Range(1, 5)]
        public int Rating { get; set; }

        /// <summary>
        /// Optional notes about the technician's performance.
        /// </summary>
        [MaxLength(2000)]
        public string? Notes { get; set; }

        /// <summary>
        /// The name of the user or system that submitted the feedback.
        /// </summary>
        [MaxLength(100)]
        public string SubmittedBy { get; set; } = string.Empty;

        /// <summary>
        /// The timestamp when the feedback was submitted.
        /// </summary>
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    }
}
