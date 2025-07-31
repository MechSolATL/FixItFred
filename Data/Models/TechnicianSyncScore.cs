using System;
using System.ComponentModel.DataAnnotations;

namespace Data.Models
{
    public enum SyncRankLevel
    {
        Bronze = 0,
        Silver = 1,
        Gold = 2,
        Platinum = 3
    }

    public class TechnicianSyncScore
    {
        [Key]
        public int Id { get; set; }
        public int TechnicianId { get; set; }
        public double SyncSuccessRate { get; set; }
        public DateTime LastUpdated { get; set; }
        public SyncRankLevel SyncRankLevel { get; set; }
        public double BonusMultiplier { get; set; }
        public DateTime? LastBonusAwarded { get; set; }
        public DateTime? CooldownUntil { get; set; }

        // Sprint 68.5: Automation UI fields
        public bool BonusEligible { get; set; }
        public int StreakLength { get; set; }
        public double CooldownRemaining { get; set; }
        public bool AutoPromoted { get; set; }
        public int RecentPenaltyCount { get; set; }
    }
}
