namespace MVP_Core.Data.Models
{
    public class UserResponse
    {
        public int Id { get; set; }
        public string SessionID { get; set; } = string.Empty; // Make it nullable if necessary
        public string Response { get; set; } = string.Empty;  // Ensure Response or Answer property exists
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
