using MVP_Core.Services;
using MVP_Core.Services.Admin;
using MVP_Core.Data.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MVP_Core.Pages.Admin
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
            TechList = _dispatcherService.GetAllTechnicianHeartbeats()?.ToList<dynamic>() ?? new List<dynamic>();
            JobList = new List<ServiceRequest>(); // TODO: Query jobs for dropdown
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var techId = int.TryParse(Request.Form["TechnicianId"], out var tId) ? tId : 0;
            var requestId = int.TryParse(Request.Form["RequestId"], out var rId) ? rId : 0;
            var notes = Request.Form["NotesOrTags"];
            var file = Request.Form.Files["MediaFile"];
            if (file == null || techId == 0 || requestId == 0)
            {
                TempData["MediaStatus"] = "Missing required fields.";
                await Task.CompletedTask;
                return Page();
            }
            notes = notes.ToString() ?? string.Empty; // FixItFred Patch: Ensure notesOrTags is not null
            var success = _mediaService.SaveMediaUpload(file, techId, requestId, User?.Identity?.Name ?? "system", notes);
            TempData["MediaStatus"] = success ? "Media uploaded successfully." : "Upload failed (invalid file or size).";
            await Task.CompletedTask; // FixItFred Patch: Ensure async compliance
            return RedirectToPage();
        }
    }
}
