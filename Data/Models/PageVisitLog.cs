namespace Data.Models
{
    public class PageVisitLog
    {
        public int Id { get; set; }

        public string PageUrl { get; set; } = string.Empty;

        public string? Referrer { get; set; }

        public string? UserAgent { get; set; }

        public string? IpAddress { get; set; }

        public bool IsRealUser { get; set; } = false;

        public int ResponseStatusCode { get; set; }

        public DateTime VisitTimestamp { get; set; } = DateTime.UtcNow;
    }
}
