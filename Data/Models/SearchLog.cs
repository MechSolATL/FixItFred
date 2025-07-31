namespace Data.Models
{
    /// <summary>
    /// Logs each public or admin search request for auditing and analytics.
    /// </summary>
    public class SearchLog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(250)]
        public string SearchTerm { get; set; } = string.Empty;

        [MaxLength(50)]
        public string IPAddress { get; set; } = "Unknown";

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
