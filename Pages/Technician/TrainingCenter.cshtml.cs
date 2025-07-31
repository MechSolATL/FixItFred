using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pages.Technician
{
    public class TrainingCenterModel : PageModel
    {
        private readonly SkillsTrackerService _skillsService;
        private readonly CertificationService _certService;
        public List<SkillTrack> AssignedTracks { get; set; } = new();
        public List<SkillProgress> Progress { get; set; } = new();
        public List<CertificationUpload> Certs { get; set; } = new();
        public bool HasRequiredMedia { get; set; } = false; // Sprint 69.0: Expose compliance status
        public TrainingCenterModel(SkillsTrackerService skillsService, CertificationService certService)
        {
            _skillsService = skillsService;
            _certService = certService;
        }
        public async Task OnGetAsync()
        {
            int techId = 0;
            if (User?.Identity?.IsAuthenticated == true && int.TryParse(User.Identity.Name, out var parsedId)) // Sprint 79.6: Harden int.Parse with TryParse
                techId = parsedId;
            AssignedTracks = _skillsService.GetAssignedTracks(techId);
            Progress = _skillsService.GetProgressForTechnician(techId);
            Certs = _certService.GetCertifications(techId);
            ServiceRequest openRequest = null; // Placeholder
            if (openRequest != null)
                HasRequiredMedia = openRequest.HasRequiredMedia;
        }
        public async Task<IActionResult> OnPostCompleteTrackAsync(int trackId)
        {
            int techId = 0;
            if (User?.Identity?.IsAuthenticated == true && int.TryParse(User.Identity.Name, out var parsedId)) // Sprint 79.6: Harden int.Parse with TryParse
                techId = parsedId;
            _skillsService.MarkTrackCompleted(techId, trackId);
            return RedirectToPage();
        }
        public async Task<IActionResult> OnPostUploadCertAsync()
        {
            int techId = 0;
            if (User?.Identity?.IsAuthenticated == true && int.TryParse(User.Identity.Name, out var parsedId)) // Sprint 79.6: Harden int.Parse with TryParse
                techId = parsedId;
            var file = Request?.Form?.Files?["certFile"];
            if (file != null && file.Length > 0)
            {
                string filePath = $"/uploads/certs/{techId}_{file.FileName}";
                using (var stream = System.IO.File.Create($"wwwroot{filePath}"))
                {
                    file.CopyTo(stream);
                }
                _certService.UploadCertification(techId, filePath, file.FileName);
            }
            return RedirectToPage();
        }
    }
}
