namespace MVP_Core.Data.Models
{
    public class RobotsContent
    {
        /// <summary>
        /// The unique identifier for the robots content entry.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The content of the robots entry.
        /// </summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// The timestamp when the robots content was last updated.
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
