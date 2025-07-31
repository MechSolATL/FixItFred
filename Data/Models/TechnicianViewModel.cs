namespace Data.Models
{
    public class TechnicianViewModel
    {
        public int Id { get; set; } // Primary identifier
        public string FullName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int AssignedJobsCount { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Specialty { get; set; }
        public string TechnicianName { get; set; } = string.Empty;
        public string CurrentTier { get; set; } = string.Empty;
        public int TotalPoints { get; set; } = 0;
        public DateTime? LastRewardSentAt { get; set; } = null;
        public bool IsEligibleForReward { get; set; } = false;
        public string Name { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
        public int Rank { get; set; } = 0;
        public int JobsCompleted { get; set; } = 0;
        public string SalesTotal { get; set; } = "$0.00";
        public double CustomerRating { get; set; } = 0.0;
        public string? Badge { get; set; } // Optional badge label
        public string? Department { get; set; } // Optional for sorting
    }
}
