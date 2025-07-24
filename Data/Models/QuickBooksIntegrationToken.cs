namespace MVP_Core.Data.Models
{
    public class QuickBooksIntegrationToken
    {
        public int Id { get; set; }
        public string CompanyId { get; set; } = string.Empty;
        public string AccessTokenRaw { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public string RealmId { get; set; } = string.Empty;
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? ExpiresAtUtc { get; set; }
    }
}
