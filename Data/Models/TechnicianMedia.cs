using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVP_Core.Data.Models
{
    /// <summary>
    /// TechnicianMedia: Stores uploaded media (images, videos, PDFs) linked to technician and job request.
    /// FileName is the stored file name. FileType is validated. Notes/Tags are for search and context.
    /// </summary>
    public class TechnicianMedia
    {
        [Key]
        public int MediaId { get; set; }
        public int TechnicianId { get; set; }
        public int RequestId { get; set; }
        [MaxLength(256)]
        public string FileName { get; set; } = string.Empty;
        [MaxLength(32)]
        public string FileType { get; set; } = string.Empty; // Image, Video, PDF, Other
        [MaxLength(100)]
        public string UploadedBy { get; set; } = string.Empty; // Name or role
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
        [MaxLength(500)]
        public string? NotesOrTags { get; set; }

        // Sprint 85.8: Photo metadata enforcement
        public double? GeoLatitude { get; set; }
        public double? GeoLongitude { get; set; }
        public DateTime? PhotoTimestamp { get; set; }
    }
}
