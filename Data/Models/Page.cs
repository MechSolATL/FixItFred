using System.ComponentModel.DataAnnotations;

namespace MVP_Core.Data.Models
{
    public class Page
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string UrlPath { get; set; } = string.Empty; // 🛠️ Correct name for the path like "/services/plumbing"

        public bool IsPublic { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}
