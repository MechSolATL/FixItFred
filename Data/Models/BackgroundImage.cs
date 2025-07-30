namespace MVP_Core.Data.Models
{
    public class BackgroundImage
    {
        /// <summary>
        /// The unique identifier for the background image.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The binary data of the image.
        /// </summary>
        [Required]
        public byte[] ImageData { get; set; } = Array.Empty<byte>();

        /// <summary>
        /// The content type of the image (e.g., image/jpeg).
        /// </summary>
        [Required]
        public string ContentType { get; set; } = "image/jpeg";

        /// <summary>
        /// The timestamp when the image was uploaded.
        /// </summary>
        [Required]
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }
}
