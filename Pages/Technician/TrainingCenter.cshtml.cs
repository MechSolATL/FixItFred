using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MVP_Core.Pages.Technician
{
    public class TrainingCenterModel : PageModel
    {
        private readonly SkillsTrackerService _skillsService;
        private readonly CertificationService _certService;
        public List<MVP_Core.Data.Models.SkillTrack> AssignedTracks { get; set; } = new();
        public List<MVP_Core.Data.Models.SkillProgress> Progress { get; set; } = new();
        public List<MVP_Core.Data.Models.CertificationUpload> Certs { get; set; } = new();
        public bool HasRequiredMedia { get; set; } = false; // Sprint 69.0: Expose compliance status
        public TrainingCenterModel(SkillsTrackerService skillsService, CertificationService certService)
        {
            _skillsService = skillsService;
            _certService = certService;
        }
        public async Task OnGetAsync()
        {
            int techId = User.Identity.IsAuthenticated ? int.Parse(User.Identity.Name) : 0;
            AssignedTracks = _skillsService.GetAssignedTracks(techId);
            Progress = _skillsService.GetProgressForTechnician(techId);
            Certs = _certService.GetCertifications(techId);
            // Get compliance status for current open ticket (pseudo: assumes one open request per tech)
            // TODO: Implement logic to get open ServiceRequest for techId
            MVP_Core.Data.Models.ServiceRequest openRequest = null; // Placeholder
            if (openRequest != null)
                HasRequiredMedia = openRequest.HasRequiredMedia;
        }
        public async Task<IActionResult> OnPostCompleteTrackAsync(int trackId)
        {
            int techId = User.Identity.IsAuthenticated ? int.Parse(User.Identity.Name) : 0;
            _skillsService.MarkTrackCompleted(techId, trackId);
            return RedirectToPage();
        }
        public async Task<IActionResult> OnPostUploadCertAsync()
        {
            int techId = User.Identity.IsAuthenticated ? int.Parse(User.Identity.Name) : 0;
            var file = Request.Form.Files["certFile"];
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
