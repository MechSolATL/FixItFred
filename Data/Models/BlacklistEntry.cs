namespace MVP_Core.Data.Models
{
    /// <summary>
    /// Represents a confirmed entry in the public or internal blacklist system.
    /// </summary>
    public class BlacklistEntry
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? PhoneNumber { get; set; }

        [MaxLength(150)]
        public string? Email { get; set; }

        [MaxLength(300)]
        public string? Reason { get; set; }

        public string? Source { get; set; }

        public string FlaggedBy { get; set; } = "System";

        public DateTime FlaggedAt { get; set; } = DateTime.UtcNow;
    }
}
