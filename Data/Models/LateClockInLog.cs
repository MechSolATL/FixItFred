using System;
using System.ComponentModel.DataAnnotations;

namespace Data.Models
{
    public class LateClockInLog
    {
        [Key]
        public int Id { get; set; }
        public int TechnicianId { get; set; }
        public DateTime ScheduledStart { get; set; }
        public DateTime ActualStart { get; set; }
        public int DelayMinutes { get; set; }
        public DateTime Date { get; set; }
        [MaxLength(20)]
        public string Severity { get; set; } = string.Empty;
    }

    public class EscalationEvent
    {
        [Key]
        public int Id { get; set; }
        public int TechnicianId { get; set; }
        public int DispatcherId { get; set; }
        public DateTime IncidentDate { get; set; }
        [MaxLength(200)]
        public string Reason { get; set; } = string.Empty;
        [MaxLength(100)]
        public string EscalatedTo { get; set; } = string.Empty;
        [MaxLength(20)]
        public string Status { get; set; } = "Open";
    }
}
