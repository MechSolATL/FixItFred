using System;

namespace Data.Models
{
    // Sprint 73.8: Roast Responder Metrics
    public class RoastReactionLog
    {
        public int Id { get; set; }
        public int RoastId { get; set; } // FK to NewHireRoastLog
        public string UserId { get; set; } = string.Empty; // Sprint 78.1: Initialized to prevent CS8618 warning
        public string ReactionType { get; set; } = string.Empty; // Sprint 78.1: Initialized to prevent CS8618 warning
        public DateTime SubmittedAt { get; set; }
    }
}
