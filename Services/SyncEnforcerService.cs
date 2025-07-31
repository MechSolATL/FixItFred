using Data;
using MVP_Core.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Services
{
    /// <summary>
    /// SyncEnforcerService: Enforces media compliance before ticket closure or job sign-off.
    /// </summary>
    public class SyncEnforcerService
    {
        private readonly ApplicationDbContext _db;
        private readonly MediaUploadService _mediaService;
        public SyncEnforcerService(ApplicationDbContext db, MediaUploadService mediaService)
        {
            _db = db;
            _mediaService = mediaService;
        }
        /// <summary>
        /// Checks compliance and blocks closure if not met. Logs incident if failed.
        /// </summary>
        public (bool IsCompliant, List<string> Errors) EnforceCompliance(int requestId, int technicianId)
        {
            var errors = new List<string>();
            var compliant = _mediaService.ValidateRequiredMedia(requestId, technicianId);
            if (!compliant)
            {
                errors.Add("Required media uploads missing or invalid (BeforeWork/AfterWork, geo, timestamp).");
                // Log incident to TechnicianPerformanceLog (pseudo, see below)
                var log = new TechnicianPerformanceLog
                {
                    TechnicianId = technicianId,
                    ServiceRequestId = requestId,
                    IncidentType = "SyncComplianceFailure",
                    Details = string.Join("; ", errors),
                    OccurredAt = DateTime.UtcNow
                };
                _db.TechnicianPerformanceLogs.Add(log);
                _db.SaveChanges();
            }
            // Update ServiceRequest compliance status
            var req = _db.ServiceRequests.FirstOrDefault(r => r.Id == requestId);
            if (req != null)
            {
                req.HasRequiredMedia = compliant;
                req.SyncComplianceCheckedAt = DateTime.UtcNow;
                _db.SaveChanges();
            }
            return (compliant, errors);
        }
    }
}
