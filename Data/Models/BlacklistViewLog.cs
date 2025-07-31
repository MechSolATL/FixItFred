namespace Data.Models
{
    public class BlacklistViewLog
    {
        public int Id { get; set; }
        public string SearchType { get; set; } = string.Empty;
        public string ServiceType { get; set; } = string.Empty;
        public string IPAddress { get; set; } = string.Empty;
        public string UserAgent { get; set; } = string.Empty;
        public string ReferrerUrl { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
