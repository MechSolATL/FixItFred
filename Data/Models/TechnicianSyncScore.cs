using System;
using System.ComponentModel.DataAnnotations;

namespace MVP_Core.Data.Models
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
    }
}
