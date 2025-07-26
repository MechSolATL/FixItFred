using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVP_Core.Data.Models
{
    public class TechnicianResponseLog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int TechnicianId { get; set; }

        [Required]
        public int RequestId { get; set; }

        [Required]
        [MaxLength(20)]
        public string ResponseSource { get; set; } = string.Empty; // "Dispatch", "Message", "JobStart"

        [Required]
        public DateTime TimeSent { get; set; }

        public DateTime? TimeAcknowledged { get; set; }

        [NotMapped]
        public int ResponseSeconds => TimeAcknowledged.HasValue ? (int)(TimeAcknowledged.Value - TimeSent).TotalSeconds : 0;

        public bool FlaggedLate { get; set; }

        public DateTime LoggedAt { get; set; }
    }
}
