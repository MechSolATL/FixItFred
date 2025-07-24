using System;
using System.ComponentModel.DataAnnotations;
using MVP_Core.Data.Models;


//namespace BROS.Web.Data.Seo
//namespace MVP_Core.Data.Models.SeoMeta
namespace MVP_Core.Data.Models


{
    public class SeoMeta
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string PageName { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? Title { get; set; }

        [MaxLength(500)]
        public string? MetaDescription { get; set; }

        [MaxLength(500)]
        public string? Keywords { get; set; }

        [MaxLength(100)]
        public string? Robots { get; set; }  // <-- Fixes CS1061 errors

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
