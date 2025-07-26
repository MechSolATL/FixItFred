using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Services;
using MVP_Core.Data.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace MVP_Core.Pages.Admin
{
    public class TechnicianCertificationsModel : PageModel
    {
        private readonly CertificationService _certService;
        public int TechId { get; set; }
        public List<CertificationUpload> Certifications { get; set; } = new();
        public List<CertificationUpload> ExpiredCerts { get; set; } = new();
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
        public IActionResult OnPostBulkApprove(int[] selectedCertIds)
        {
            foreach (var id in selectedCertIds)
            {
                _certService.VerifyCertification(id);
            }
            return RedirectToPage(new { techId = TechId });
        }
        public IActionResult OnPostBulkReject(int[] selectedCertIds)
        {
            foreach (var id in selectedCertIds)
            {
                _certService.RejectCertificationAsync(id, "Bulk Rejected");
            }
            return RedirectToPage(new { techId = TechId });
        }
        public IActionResult OnPostExportCsv(int[] selectedCertIds)
        {
            var certs = Certifications.Where(c => selectedCertIds.Contains(c.Id)).ToList();
            var csv = "Technician,LicenseNumber,IssueDate,ExpiryDate,VerificationStatus\n" +
                string.Join("\n", certs.Select(c => $"{c.TechnicianId},{c.LicenseNumber},{c.IssueDate:yyyy-MM-dd},{(c.ExpiryDate.HasValue ? c.ExpiryDate.Value.ToString("yyyy-MM-dd") : "")},{c.VerificationStatus}"));
            var bytes = System.Text.Encoding.UTF8.GetBytes(csv);
            return File(bytes, "text/csv", "certifications.csv");
        }
    }
}
