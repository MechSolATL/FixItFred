// [Sprint91_27] Backward compatibility namespace for Razor pages
// This ensures existing @inject Services.* directives continue to work

using Data.Models.Seo;

namespace Services
{
    /// <summary>
    /// Backward compatibility interface for ISeoService
    /// </summary>
    public interface ISeoService
    {
        Task<string> GetSeoForPageAsync(string pageName);
        Task<string> GetSeoByPageNameAsync(string pageName);
        Task<SeoMeta> GetSeoMetaAsync(string pageName);
    }
    
    /// <summary>
    /// Backward compatibility implementation for SEOService
    /// </summary>
    public class SEOService : ISeoService
    {
        private readonly MVP_Core.Services.ISeoService _seoService;
        
        public SEOService(MVP_Core.Services.ISeoService seoService)
        {
            _seoService = seoService;
        }
        
        public Task<SeoMeta> GetSeoMetaAsync(string pageName)
        {
            return _seoService.GetSeoMetaAsync(pageName);
        }

        public Task<string> GetSeoByPageNameAsync(string pageName)
        {
            return _seoService.GetSeoByPageNameAsync(pageName);
        }

        public async Task<string> GetSeoForPageAsync(string pageName)
        {
            return await _seoService.GetSeoForPageAsync(pageName);
        }
    }
    
    /// <summary>
    /// Backward compatibility implementation for CertificationService
    /// </summary>
    public class CertificationService
    {
        private readonly MVP_Core.Services.CertificationService _certificationService;
        
        public CertificationService(MVP_Core.Services.CertificationService certificationService)
        {
            _certificationService = certificationService;
        }
        
        // Delegate methods using the correct model types and method signatures
        public List<Data.Models.CertificationUpload> GetCertifications(int techId) => _certificationService.GetCertifications(techId);
        public Task<List<Data.Models.CertificationUpload>> GetCertificationsAsync(int techId) => _certificationService.GetCertificationsAsync(techId);
        public Task<List<Data.Models.CertificationUpload>> GetCertificationsByTechnicianAsync(int techId) => _certificationService.GetCertificationsByTechnicianAsync(techId);
        public List<Data.Models.CertificationUpload> GetExpiredCertifications() => _certificationService.GetExpiredCertifications();
        public List<Data.Models.CertificationUpload> GetPendingVerifications() => _certificationService.GetPendingVerifications();
        public void VerifyCertification(int certId) => _certificationService.VerifyCertification(certId);
        public void VerifyCertification(int certId, string reviewerName) => _certificationService.VerifyCertification(certId, reviewerName);
        public Task VerifyCertificationAsync(int certId) => _certificationService.VerifyCertificationAsync(certId);
        public Task RejectCertificationAsync(int certId, string reason) => _certificationService.RejectCertificationAsync(certId, reason);
        public void UploadCertification(int technicianId, string filePath, string certName = "", string licenseNumber = "", DateTime? issueDate = null, DateTime? expiryDate = null) => _certificationService.UploadCertification(technicianId, filePath, certName, licenseNumber, issueDate, expiryDate);
        public void MarkExpiredCerts() => _certificationService.MarkExpiredCerts();
    }
    
    /// <summary>
    /// Backward compatibility implementation for SkillsTrackerService
    /// </summary>
    public class SkillsTrackerService
    {
        private readonly MVP_Core.Services.SkillsTrackerService _skillsTrackerService;
        
        public SkillsTrackerService(MVP_Core.Services.SkillsTrackerService skillsTrackerService)
        {
            _skillsTrackerService = skillsTrackerService;
        }
        
        // Delegate methods using the correct model types and method signatures that actually exist
        public List<Data.Models.SkillTrack> GetAssignedTracks(int technicianId) => _skillsTrackerService.GetAssignedTracks(technicianId);
        public List<Data.Models.SkillProgress> GetProgressForTechnician(int technicianId) => _skillsTrackerService.GetProgressForTechnician(technicianId);
        public List<Data.Models.SkillTrack> GetAllTracks() => _skillsTrackerService.GetAllTracks();
        public List<Data.Models.Technician> GetAllTechnicians() => _skillsTrackerService.GetAllTechnicians();
        public bool AssignTrack(int technicianId, int skillTrackId) => _skillsTrackerService.AssignTrack(technicianId, skillTrackId);
        public bool MarkTrackCompleted(int technicianId, int skillTrackId) => _skillsTrackerService.MarkTrackCompleted(technicianId, skillTrackId);
        public bool IsEligibleForBadge(int technicianId) => _skillsTrackerService.IsEligibleForBadge(technicianId);
    }
    
    /// <summary>
    /// Backward compatibility implementation for ContentService
    /// </summary>
    public interface IContentService
    {
        Task<string> GetByKeyAsync(string key);
        Task<string> GetContentAsync(string key);
    }
    
    /// <summary>
    /// Backward compatibility implementation for ContentService
    /// </summary>
    public class ContentService : IContentService
    {
        private readonly MVP_Core.Services.IContentService _contentService;
        
        public ContentService(MVP_Core.Services.IContentService contentService)
        {
            _contentService = contentService;
        }
        
        public Task<string> GetByKeyAsync(string key) => _contentService.GetByKeyAsync(key);
        public Task<string> GetContentAsync(string key) => _contentService.GetContentAsync(key);
    }
}