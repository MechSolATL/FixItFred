namespace MVP_Core.Data.Models
{
    public class PendingEntry
    {
        public int Id { get; set; }
        public string TryType { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Status { get; set; } = "New";
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    }
}
