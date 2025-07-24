namespace MVP_Core.Data.Models
{
    public class RobotsContent
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
