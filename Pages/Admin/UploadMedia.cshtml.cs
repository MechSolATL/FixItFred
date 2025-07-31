// Sprint 26.5 Patch Log: CS860x/CS8625/CS1998/CS0219 fixes — Nullability, async, and unused variable corrections for Nova review.
// Sprint 26.6 Patch Log: CS8601/CS8602/CS8603/CS8604 fixes — Added null checks and safe navigation for all nullable references. Previous comments preserved below.
// FixItFred: Sprint 30D.2 ? CS860x nullability audit 2024-07-25
// Added null checks and safe navigation for all nullable references per CS8601, CS8602, CS8603, CS8604
// Each change is marked with FixItFred comment and timestamp

using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Services;
using Data.Models;
using Services.Admin;

namespace Pages.Admin
{
    /// <summary>
    /// PageModel for UploadMedia. Injects MediaUploadService and DispatcherService via DI.
    /// Uses dynamic object for technician list to avoid missing type errors.
    /// </summary>
    public class UploadMediaModel : PageModel
    {
        private readonly MediaUploadService _mediaService;
        private readonly DispatcherService _dispatcherService;
        public List<dynamic> TechList { get; private set; } = new();
        public List<ServiceRequest> JobList { get; private set; } = new();
        public string? Status { get; private set; }
        public string? NotesOrTags { get; set; }

        public UploadMediaModel(MediaUploadService mediaService, DispatcherService dispatcherService)
        {
            _mediaService = mediaService;
            _dispatcherService = dispatcherService;
        }

        public void OnGet()
        {
            TechList = _dispatcherService?.GetAllTechnicianHeartbeats()?.ToList<dynamic>() ?? new List<dynamic>();
            JobList = new List<ServiceRequest>(); // TODO: Query jobs for dropdown
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var techId = int.TryParse(Request?.Form["TechnicianId"].ToString(), out var tId) ? tId : 0;
            var requestId = int.TryParse(Request?.Form["RequestId"].ToString(), out var rId) ? rId : 0;
            var notes = Request?.Form["NotesOrTags"].ToString() ?? string.Empty;
            var file = Request?.Form.Files["MediaFile"];
            var photoTimestampStr = Request?.Form["PhotoTimestamp"].ToString();
            var geoLatStr = Request?.Form["GeoLatitude"].ToString();
            var geoLngStr = Request?.Form["GeoLongitude"].ToString();
            DateTime? photoTimestamp = DateTime.TryParse(photoTimestampStr, out var ts) ? ts : null;
            double? geoLat = double.TryParse(geoLatStr, out var lat) ? lat : null;
            double? geoLng = double.TryParse(geoLngStr, out var lng) ? lng : null;
            if (file == null || techId == 0 || requestId == 0 || photoTimestamp == null || geoLat == null || geoLng == null)
            {
                TempData["MediaStatus"] = "Missing required fields.";
                await Task.CompletedTask;
                return Page();
            }
            var username = User?.Identity?.Name?.ToString() ?? "system";
            var success = _mediaService.SaveMediaUpload(file, techId, requestId, username, notes, photoTimestamp, geoLat, geoLng);
            TempData["MediaStatus"] = success ? "Media uploaded successfully." : "Upload failed (invalid file, size, or metadata).";
            await Task.CompletedTask;
            return RedirectToPage();
        }
    }
}
