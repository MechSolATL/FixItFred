namespace MVP_Core.Models.Admin
{
    public class DispatcherBroadcast
    {
        public int Id { get; set; }
        public required string Message { get; set; }
        public DateTime IssuedAt { get; set; }
        public required string IssuedBy { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public bool IsActive => !ExpiresAt.HasValue || ExpiresAt > DateTime.UtcNow;
    }
}
