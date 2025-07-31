using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models
{
    public class TechnicianWarningLog
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Technician")]
        public int TechnicianId { get; set; }
        [MaxLength(50)]
        public string TriggerCode { get; set; } = string.Empty;
        [MaxLength(50)]
        public string SourceZone { get; set; } = string.Empty;
        public int? RelatedRequestId { get; set; }
        public DateTime Timestamp { get; set; }
        [MaxLength(20)]
        public string Severity { get; set; } = "Info";
        public bool ResolvedFlag { get; set; } = false;
        [MaxLength(100)]
        public string Reason { get; set; } = string.Empty;

        public Technician? Technician { get; set; }
    }
}
