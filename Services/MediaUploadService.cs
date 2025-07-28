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
        public bool SaveMediaUpload(IFormFile file, int technicianId, int requestId, string uploadedBy, string notesOrTags, DateTime? photoTimestamp = null, double? geoLat = null, double? geoLng = null)
        {
            // Sprint 80: IFormFile and parameter hardening
            if (file == null || string.IsNullOrWhiteSpace(uploadedBy) || photoTimestamp == null || geoLat == null || geoLng == null)
                return false;
            uploadedBy = uploadedBy ?? "System";
            notesOrTags = notesOrTags ?? string.Empty;
            var allowedTypes = new[] { ".jpg", ".jpeg", ".png", ".gif", ".pdf", ".mp4" };
            var ext = Path.GetExtension(file.FileName ?? string.Empty).ToLowerInvariant();
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
            try
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                    file.CopyTo(stream);
            }
            catch
            {
                return false;
            }
            var media = new TechnicianMedia
            {
                TechnicianId = technicianId,
                RequestId = requestId,
                FileName = safeFileName,
                FileType = fileType,
                UploadedBy = uploadedBy,
                UploadedAt = DateTime.UtcNow,
                NotesOrTags = notesOrTags,
                PhotoTimestamp = photoTimestamp,
                GeoLatitude = geoLat,
                GeoLongitude = geoLng
            };
            _db.TechnicianMedias.Add(media);
            _db.SaveChanges();
            // Fraud/metadata validation
            var incidentDetails = string.Empty;
            if (photoTimestamp > DateTime.UtcNow.AddMinutes(5) || photoTimestamp < DateTime.UtcNow.AddYears(-1))
                incidentDetails += "Photo timestamp is suspicious. ";
            if (geoLat < -90 || geoLat > 90 || geoLng < -180 || geoLng > 180)
                incidentDetails += "GeoLocation is out of bounds. ";
            if (!string.IsNullOrEmpty(incidentDetails))
            {
                _db.TechnicianPerformanceLogs.Add(new TechnicianPerformanceLog
                {
                    TechnicianId = technicianId,
                    ServiceRequestId = requestId,
                    IncidentType = "PhotoMetadataDiscrepancy",
                    Details = incidentDetails,
                    OccurredAt = DateTime.UtcNow
                });
                _db.SaveChanges();
            }
            return true;
        }
        public List<TechnicianMedia> GetMediaForRequest(int requestId)
            => _db.TechnicianMedias.Where(m => m.RequestId == requestId).OrderByDescending(m => m.UploadedAt).ToList();
        public List<TechnicianMedia> GetMediaForTechnician(int techId)
            => _db.TechnicianMedias.Where(m => m.TechnicianId == techId).OrderByDescending(m => m.UploadedAt).ToList();
        /// <summary>
        /// Validates that required media (BeforeWork and AfterWork photos) are present and compliant for a service request.
        /// </summary>
        /// <param name="requestId">ServiceRequest ID</param>
        /// <param name="technicianId">Technician ID</param>
        /// <returns>True if compliant, false otherwise</returns>
        public bool ValidateRequiredMedia(int requestId, int technicianId)
        {
            var media = _db.TechnicianMedias.Where(m => m.RequestId == requestId && m.TechnicianId == technicianId).ToList();
            // Must have at least one BeforeWork and one AfterWork photo
            bool hasBefore = media.Any(m => m.NotesOrTags != null && m.NotesOrTags.Contains("BeforeWork", StringComparison.OrdinalIgnoreCase) && m.FileType == "Image");
            bool hasAfter = media.Any(m => m.NotesOrTags != null && m.NotesOrTags.Contains("AfterWork", StringComparison.OrdinalIgnoreCase) && m.FileType == "Image");
            if (!hasBefore || !hasAfter)
                return false;
            // Check geo-matching and backdating (pseudo, assumes NotesOrTags contains geo info and timestamps are valid)
            foreach (var m in media.Where(x => x.NotesOrTags != null && (x.NotesOrTags.Contains("BeforeWork") || x.NotesOrTags.Contains("AfterWork"))))
            {
                // Example: NotesOrTags contains "Geo:VALID" if geo-matched
                if (!m.NotesOrTags.Contains("Geo:VALID"))
                    return false;
                // UploadedAt must not be backdated (not before request creation)
                var request = _db.ServiceRequests.FirstOrDefault(r => r.Id == requestId);
                if (request != null && m.UploadedAt < request.CreatedAt)
                    return false;
            }
            return true;
        }
    }
}
