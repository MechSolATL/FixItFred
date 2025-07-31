using System;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    // Sprint 86.6 — Mentor Reply Log
    public class MentorReplyLog
    {
        [Key]
        public int Id { get; set; }
        public int MentorId { get; set; }
        public int RequestingUserId { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string ResponseText { get; set; } = string.Empty;
        public int FlowId { get; set; }
        public int? HelpfulnessRating { get; set; }
    }
}
