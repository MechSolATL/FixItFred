// Sprint 34.2 - SLA Escalation Log Model
using System;
using System.ComponentModel.DataAnnotations;

namespace MVP_Core.Data.Models
{
    public class EscalationLogEntry
    {
        [Key]
        public int Id { get; set; }
        public int ScheduleQueueId { get; set; }
        public string? TriggeredBy { get; set; }
        public string? Reason { get; set; }
        public string? ActionTaken { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
