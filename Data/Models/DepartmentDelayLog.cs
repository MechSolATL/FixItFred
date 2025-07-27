using System;
using System.ComponentModel.DataAnnotations;

namespace MVP_Core.Data.Models
{
    public class DepartmentDelayLog
    {
        public int Id { get; set; }
        [Required]
        public string Department { get; set; } = string.Empty;
        public DateTime DelayStart { get; set; }
        public DateTime? DelayEnd { get; set; }
        public int? EscalationLevel { get; set; }
        public string? EscalationReason { get; set; }
        public DateTime? ResolutionTime { get; set; }
        public string? ResolutionNotes { get; set; }
        public int Severity { get; set; }
        public int? RelatedTaskId { get; set; }
    }
}
