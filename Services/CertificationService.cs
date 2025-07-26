using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        public List<CertificationRecord> GetCertifications(int technicianId)
        {
            return _db.CertificationRecords.Where(c => c.TechnicianId == technicianId).OrderByDescending(c => c.ExpiryDate).ToList();
        }
        public void UploadCertification(int technicianId, string name, string licenseNumber, DateTime issueDate, DateTime expiryDate, string documentPath)
        {
            var cert = new CertificationRecord
            {
                TechnicianId = technicianId,
                CertificationName = name,
                LicenseNumber = licenseNumber,
                IssueDate = issueDate,
                ExpiryDate = expiryDate,
                DocumentPath = documentPath,
                IsVerified = false,
                CreatedAt = DateTime.UtcNow
            };
            _db.CertificationRecords.Add(cert);
            _db.SaveChanges();
        }
        public List<CertificationRecord> GetExpiredCertifications()
        {
            return _db.CertificationRecords.Where(c => c.ExpiryDate < DateTime.UtcNow).ToList();
        }
        public void VerifyCertification(int certId)
        {
            var cert = _db.CertificationRecords.FirstOrDefault(c => c.Id == certId);
            if (cert != null)
            {
                cert.IsVerified = true;
                _db.SaveChanges();
            }
        }
    }
}