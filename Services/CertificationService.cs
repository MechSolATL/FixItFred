using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MVP_Core.Data;
using MVP_Core.Data.Models;

namespace MVP_Core.Services
{
    public class CertificationService
    {
        private readonly ApplicationDbContext _db;
        public CertificationService(ApplicationDbContext db)
        {
            _db = db;
        }
        public List<CertificationUpload> GetCertifications(int techId)
        {
            return _db.CertificationUploads.Where(c => c.TechnicianId == techId).ToList();
        }
        public async Task<List<CertificationUpload>> GetCertificationsAsync(int techId)
        {
            return await Task.FromResult(_db.CertificationUploads.Where(c => c.TechnicianId == techId).ToList());
        }
        public List<CertificationUpload> GetExpiredCertifications()
        {
            var now = DateTime.UtcNow;
            return _db.CertificationUploads.Where(c => c.ExpiryDate != null && c.ExpiryDate < now).ToList();
        }
        public void VerifyCertification(int certId)
        {
            var cert = _db.CertificationUploads.FirstOrDefault(c => c.Id == certId);
            if (cert != null)
            {
                cert.VerificationStatus = "Verified";
                cert.VerifiedBy = "Admin";
                _db.SaveChanges();
            }
        }
        public void VerifyCertification(int certId, string reviewerName)
        {
            var cert = _db.CertificationUploads.FirstOrDefault(c => c.Id == certId);
            if (cert != null)
            {
                cert.VerificationStatus = "Verified";
                cert.VerifiedBy = reviewerName;
                _db.SaveChanges();
            }
        }
        public async Task<List<CertificationUpload>> GetCertificationsByTechnicianAsync(int techId)
        {
            return await Task.FromResult(_db.CertificationUploads.Where(c => c.TechnicianId == techId).ToList());
        }
        public async Task VerifyCertificationAsync(int certId)
        {
            var cert = _db.CertificationUploads.FirstOrDefault(c => c.Id == certId);
            if (cert != null)
            {
                cert.VerificationStatus = "Verified";
                cert.VerifiedBy = "Admin";
                _db.SaveChanges();
            }
            await Task.CompletedTask;
        }
        public async Task RejectCertificationAsync(int certId, string reason)
        {
            var cert = _db.CertificationUploads.FirstOrDefault(c => c.Id == certId);
            if (cert != null)
            {
                cert.VerificationStatus = "Rejected";
                cert.VerifiedBy = reason;
                _db.SaveChanges();
            }
            await Task.CompletedTask;
        }
        public List<CertificationUpload> GetPendingVerifications()
        {
            return _db.CertificationUploads.Where(c => c.VerificationStatus == "Pending").ToList();
        }
        public void UploadCertification(int technicianId, string filePath, string certName = "", string licenseNumber = "", DateTime? issueDate = null, DateTime? expiryDate = null)
        {
            _db.CertificationUploads.Add(new CertificationUpload
            {
                TechnicianId = technicianId,
                FilePath = filePath,
                CertificationName = certName,
                LicenseNumber = licenseNumber,
                IssueDate = issueDate ?? DateTime.UtcNow,
                ExpiryDate = expiryDate,
                VerificationStatus = "Pending"
            });
            _db.SaveChanges();
        }
        public void MarkExpiredCerts()
        {
            var now = DateTime.UtcNow;
            var expired = _db.CertificationUploads.Where(c => c.ExpiryDate != null && c.ExpiryDate < now && c.VerificationStatus != "Expired").ToList();
            foreach (var cert in expired)
            {
                cert.VerificationStatus = "Expired";
            }
            _db.SaveChanges();
        }
    }
}