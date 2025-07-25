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
        [Key]
        public int FeedbackId { get; set; }
        public int TechnicianId { get; set; }
        public int RequestId { get; set; }
        [Range(1, 5)]
        public int Rating { get; set; } // 1=poor, 5=excellent
        [MaxLength(2000)]
        public string? Notes { get; set; }
        [MaxLength(100)]
        public string SubmittedBy { get; set; } = string.Empty;
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    }
}
