using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MVP_Core.Pages.Admin
{
    public class SkillsTrackerModel : PageModel
    {
        private readonly SkillsTrackerService _skillsService;
        private readonly CertificationService _certService;
        public List<MVP_Core.Data.Models.SkillTrack> AllTracks { get; set; } = new();
        public List<MVP_Core.Data.Models.Technician> AllTechs { get; set; } = new();
        public List<MVP_Core.Data.Models.CertificationUpload> PendingCerts { get; set; } = new();
        public Dictionary<int, List<MVP_Core.Data.Models.SkillProgress>> TechProgress { get; set; } = new();
        public List<MVP_Core.Data.Models.CertificationUpload> ExpiredCerts { get; set; } = new();
        public SkillsTrackerModel(SkillsTrackerService skillsService, CertificationService certService)
        {
            _skillsService = skillsService;
            _certService = certService;
        }
        public async Task OnGetAsync()
        {
            AllTracks = _skillsService.GetAllTracks();
            AllTechs = _skillsService.GetAllTechnicians();
            PendingCerts = _certService.GetPendingVerifications();
            ExpiredCerts = _certService.GetExpiredCertifications();
            TechProgress = new();
            foreach (var tech in AllTechs)
            {
                TechProgress[tech.Id] = _skillsService.GetProgressForTechnician(tech.Id);
            }
        }
        public async Task<IActionResult> OnPostAssignTrackAsync(int techId, int trackId)
        {
            _skillsService.AssignTrack(techId, trackId);
            return RedirectToPage();
        }
        public async Task<IActionResult> OnPostVerifyCertAsync(int certId)
        {
            _certService.VerifyCertification(certId);
            return RedirectToPage();
        }
    }
}
