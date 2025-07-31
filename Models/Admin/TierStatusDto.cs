using System;

namespace Models.Admin
{
    // Sprint 84.2: Technician Tier Status DTO
    public class TierStatusDto
    {
        public int TechnicianId { get; set; }
        public string TechnicianName { get; set; } = string.Empty;
        public string CurrentTier { get; set; } = string.Empty;
        public int TotalPoints { get; set; }
        public DateTime? LastRewardSentAt { get; set; }
        public bool IsEligibleForReward { get; set; }
    }
}
