using System;
using System.ComponentModel.DataAnnotations;

namespace MVP_Core.Data.Models
{
    public class BreakComplianceOverrideLog
    {
        [Key]
        public int Id { get; set; }
        public int TechnicianId { get; set; }
        public int ApprovedByUserId { get; set; }
        [MaxLength(100)]
        public string RoleOfApprover { get; set; } = string.Empty;
        [MaxLength(2000)]
        public string Justification { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public int DurationExtended { get; set; } // Minutes
        public bool WasEmergency { get; set; }
        public bool AutoFlagged { get; set; }
    }
}
