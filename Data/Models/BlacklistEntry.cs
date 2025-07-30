namespace MVP_Core.Data.Models
{
    /// <summary>
    /// Represents a confirmed entry in the public or internal blacklist system.
    /// </summary>
    public class BlacklistEntry
    {
        /// <summary>
        /// The unique identifier for the blacklist entry.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The name of the individual or entity being blacklisted.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The phone number associated with the blacklist entry.
        /// </summary>
        [MaxLength(100)]
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// The email address associated with the blacklist entry.
        /// </summary>
        [MaxLength(150)]
        public string? Email { get; set; }

        /// <summary>
        /// The reason for blacklisting the individual or entity.
        /// </summary>
        [MaxLength(300)]
        public string? Reason { get; set; }

        /// <summary>
        /// The source of the blacklist entry (e.g., internal system, external report).
        /// </summary>
        public string? Source { get; set; }

        /// <summary>
        /// The user or system that flagged the entry.
        /// </summary>
        public string FlaggedBy { get; set; } = "System";

        /// <summary>
        /// The timestamp when the entry was flagged.
        /// </summary>
        public DateTime FlaggedAt { get; set; } = DateTime.UtcNow;
    }
}
