namespace Data.Models
{
    public class TechnicianViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int AssignedJobsCount { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Specialty { get; set; }
        
        // Additional properties for leaderboard functionality
        public string TechnicianName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string CurrentTier { get; set; } = string.Empty;
        public int TotalPoints { get; set; }
        public DateTime? LastRewardSentAt { get; set; }
        public bool IsEligibleForReward { get; set; }
        public string Avatar { get; set; } = "/images/default-avatar.png";
        public int Rank { get; set; }
        public int JobsCompleted { get; set; }
        public decimal SalesTotal { get; set; }
    }
}
