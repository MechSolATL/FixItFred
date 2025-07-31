// Sprint 34.2 - SLA Escalation Log Model
using System;
using System.ComponentModel.DataAnnotations;

namespace Data.Models
{
    public class EscalationLogEntry
    {
        /// <summary>
        /// The unique identifier for the escalation log entry.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The ID of the associated schedule queue.
        /// </summary>
        public int ScheduleQueueId { get; set; }

        /// <summary>
        /// The user or system that triggered the escalation.
        /// </summary>
        public string? TriggeredBy { get; set; }

        /// <summary>
        /// The reason for the escalation.
        /// </summary>
        public string? Reason { get; set; }

        /// <summary>
        /// The action taken in response to the escalation.
        /// </summary>
        public string? ActionTaken { get; set; }

        /// <summary>
        /// The timestamp when the escalation log entry was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}
