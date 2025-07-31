namespace Data.Models
{
    /// <summary>
    /// Represents a log entry for token-related actions, such as retrieval, refresh, or expiration.
    /// </summary>
    public class TokenLog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string TokenType { get; set; } = string.Empty;

        [Required]
        public string TokenValue { get; set; } = string.Empty;

        [Required]
        public string Action { get; set; } = string.Empty;

        public string UserName { get; set; } = "System"; // Default for safety

        public DateTime Issued { get; set; } = DateTime.UtcNow;

        public DateTime Expires { get; set; } = DateTime.UtcNow.AddHours(1);

        public DateTime LoggedAt { get; set; } = DateTime.UtcNow;

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public string? PerformedBy { get; set; }
    }
}
