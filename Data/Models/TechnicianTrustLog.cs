using System;

namespace MVP_Core.Data.Models
{
    public class TechnicianTrustLog
    {
        public int Id { get; set; }
        public int TechnicianId { get; set; }
        public decimal TrustScore { get; set; }
        public decimal FlagWeight { get; set; }
        public DateTime RecordedAt { get; set; }
        // Sprint 84.2 — Reward Trigger Engine
        public DateTime? LastRewardSentAt { get; set; }
    }
}
