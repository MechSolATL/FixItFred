namespace MVP_Core.Data.Models
{
    public class FlaggedCustomer
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string FlaggedBy { get; set; } = string.Empty;
        public string FlaggedReason { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime FlaggedAt { get; set; } = DateTime.UtcNow;
        public string UserAgent { get; set; } = string.Empty;
    }
}
