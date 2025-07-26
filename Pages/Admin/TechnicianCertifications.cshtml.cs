using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Services;
using MVP_Core.Data.Models;
using System.Collections.Generic;

namespace MVP_Core.Pages.Admin
{
    public class TechnicianCertificationsModel : PageModel
    {
        private readonly CertificationService _certService;
        public int TechId { get; set; }
        public List<CertificationRecord> Certifications { get; set; } = new();
        public List<CertificationRecord> ExpiredCerts { get; set; } = new();
        public TechnicianCertificationsModel(CertificationService certService)
        {
            _certService = certService;
        }
        public void OnGet(int techId)
        {
            TechId = techId;
            if (techId > 0)
                Certifications = _certService.GetCertifications(techId);
            ExpiredCerts = _certService.GetExpiredCertifications();
        }
        public void OnPost(int certId)
        {
            _certService.VerifyCertification(certId);
        }
    }
}
