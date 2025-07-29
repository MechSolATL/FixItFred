using System;

namespace MVP_Core.Data.Models.PatchAnalytics
{
    public class TechnicianBadge
    {
        public int Id { get; set; }
        public int TechnicianId { get; set; }
        public string BadgeType { get; set; } = string.Empty; // e.g. "TopShout", "StreakMaster", "PatchFav"
        public DateTime EarnedOn { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? IconPath { get; set; }
    }
}
