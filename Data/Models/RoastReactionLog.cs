using System;

namespace MVP_Core.Data.Models
{
    // Sprint 73.8: Roast Responder Metrics
    public class RoastReactionLog
    {
        public int Id { get; set; }
        public int RoastId { get; set; } // FK to NewHireRoastLog
        public string UserId { get; set; } // User who reacted
        public string ReactionType { get; set; } // Emoji, star, etc.
        public DateTime SubmittedAt { get; set; }
    }
}
