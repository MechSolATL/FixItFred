namespace MVP_Core.Data.Models
{
    public class UserResponse
    {
        public int Id { get; set; }

        public string SessionID { get; set; } = string.Empty;

        public int QuestionId { get; set; } // NEW: ties to dynamic questions

        public string Response { get; set; } = string.Empty;

        public string? ServiceType { get; set; } // Optional but useful

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
