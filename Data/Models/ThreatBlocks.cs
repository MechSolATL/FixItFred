namespace MVP_Core.Data.Models
{
    public class ThreatBlock
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string IpAddress { get; set; } = string.Empty;

        [MaxLength(500)]
        public string UserAgent { get; set; } = string.Empty;

        public int StrikeCount { get; set; } = 0;

        public bool IsPermanentlyBlocked { get; set; } = false;

        public DateTime FirstDetectedAt { get; set; } = DateTime.UtcNow;

        public DateTime LastDetectedAt { get; set; } = DateTime.UtcNow;

        public DateTime? BanLiftTime { get; set; } = null;
    }
}
