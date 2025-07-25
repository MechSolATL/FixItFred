using MVP_Core.Data.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MVP_Core.Services
{
    /// <summary>
    /// MediaUploadService: Handles saving media files to /wwwroot/media/{requestId}/ and tracking in DB.
    /// Validates file type and size. Database stores metadata, files stored on disk.
    /// </summary>
    public class MediaUploadService
    {
        private readonly ApplicationDbContext _db;
        private readonly string _mediaRoot;
        public MediaUploadService(ApplicationDbContext db)
        {
            _db = db;
            _mediaRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "media");
        }
        public bool SaveMediaUpload(IFormFile file, int technicianId, int requestId, string uploadedBy, string notesOrTags)
        {
            // Validate file type and size
            var allowedTypes = new[] { ".jpg", ".jpeg", ".png", ".gif", ".pdf", ".mp4" };
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedTypes.Contains(ext) || file.Length > 10 * 1024 * 1024)
                return false;
            var fileType = ext switch
            {
                ".jpg" or ".jpeg" or ".png" or ".gif" => "Image",
                ".pdf" => "PDF",
                ".mp4" => "Video",
                _ => "Other"
            };
            var requestDir = Path.Combine(_mediaRoot, requestId.ToString());
            Directory.CreateDirectory(requestDir);
            var safeFileName = Guid.NewGuid().ToString("N") + ext;
            var filePath = Path.Combine(requestDir, safeFileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
                file.CopyTo(stream);
            var media = new TechnicianMedia
            {
                TechnicianId = technicianId,
                RequestId = requestId,
                FileName = safeFileName,
                FileType = fileType,
                UploadedBy = uploadedBy,
                UploadedAt = DateTime.UtcNow,
                NotesOrTags = notesOrTags
            };
            _db.TechnicianMedias.Add(media);
            _db.SaveChanges();
            return true;
        }
        public List<TechnicianMedia> GetMediaForRequest(int requestId)
            => _db.TechnicianMedias.Where(m => m.RequestId == requestId).OrderByDescending(m => m.UploadedAt).ToList();
        public List<TechnicianMedia> GetMediaForTechnician(int techId)
            => _db.TechnicianMedias.Where(m => m.TechnicianId == techId).OrderByDescending(m => m.UploadedAt).ToList();
    }
}
