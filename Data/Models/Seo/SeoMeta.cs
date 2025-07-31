// [FixItFred - Sprint 91.23.A] Confirmed SeoMeta.cs as the authoritative model. Duplicate removed from root Models folder.

using System.ComponentModel.DataAnnotations;

namespace Data.Models.Seo
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
