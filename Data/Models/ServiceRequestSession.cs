using System;
using System.Collections.Generic;

namespace MVP_Core.Models
{
    public class ServiceRequestSession
    {
        public int Version { get; set; } = 1;  // 🔥 For future-proof session deserialization upgrades

        public string ServiceType { get; set; } = "Plumbing";

        // QuestionId → (Answer text, Answered timestamp)
        public Dictionary<int, (string Answer, DateTime AnsweredAt)> Answers { get; set; } = new();

        public string CustomerName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;

        public bool PhoneVerified { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;  // 🔥 Always know when the session started
        public DateTime? ExpiresAt { get; set; }   // (Optional) Allow session-level expiry hints if needed later
    }
}
