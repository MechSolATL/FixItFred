namespace MVP_Core.Data.Models
{
    public class SessionInfo
    {
        public int Id { get; set; }
        public string SessionID { get; set; } = string.Empty; // Ensures non-null default value
        public string IpAddress { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
