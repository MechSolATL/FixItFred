using System.ComponentModel.DataAnnotations;

namespace MVP_Core.Data.Models
{
    public class SeoMeta
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [MaxLength(60, ErrorMessage = "Title should not exceed 60 characters.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Meta description is required.")]
        [MaxLength(160, ErrorMessage = "Meta description should not exceed 160 characters.")]
        public string MetaDescription { get; set; } = string.Empty;

        [MaxLength(255, ErrorMessage = "Keywords should not exceed 255 characters.")]
        public string Keywords { get; set; } = string.Empty;

        [Required(ErrorMessage = "Page name is required.")]
        public string PageName { get; set; } = string.Empty;

        [MaxLength(25, ErrorMessage = "Robots meta tag should not exceed 25 characters.")]
        public string Robots { get; set; } = "index, follow";
    }
}
