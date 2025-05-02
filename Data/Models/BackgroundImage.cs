using System.ComponentModel.DataAnnotations;

namespace MVP_Core.Data.Models
{
    public class BackgroundImage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public byte[] ImageData { get; set; } = Array.Empty<byte>();

        [Required]
        public string ContentType { get; set; } = "image/jpeg";

        [Required]
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }
}
