using MVP_Core.Data.Models; // Add this if missing


namespace MVP_Core.Data.Models
{
    public class ServiceRequest
    {
        public int Id { get; set; }
        public required string CustomerName { get; set; }
        public required string Email { get; set; }
        public required string ServiceType { get; set; }
        public required string Details { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public required string Status { get; set; } = "Pending";
    }
}
