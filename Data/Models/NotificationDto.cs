namespace Data.Models
{
    // Sprint 83.4: Stub model to resolve CS0246 in DispatcherService
    public class NotificationDto
    {
        public int Id { get; set; }
        public string Message { get; set; } = string.Empty;
        public string SentBy { get; set; } = string.Empty;
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
    }
}
