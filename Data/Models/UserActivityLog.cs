namespace Data.Models
{
    public class UserActivityLog
    {
        public int Id { get; set; }
        public string UserId { get; set; } = "Anonymous";
        public string SessionId { get; set; } = string.Empty;
        public string IPAddress { get; set; } = string.Empty;
        public string UserAgent { get; set; } = string.Empty;
        public string PathAccessed { get; set; } = string.Empty;
        public string InteractionType { get; set; } = "View";
        public DateTime EntryTimeUtc { get; set; } = DateTime.UtcNow;
        public DateTime? ExitTimeUtc { get; set; }
        public int? DurationSeconds { get; set; }
    }
}
