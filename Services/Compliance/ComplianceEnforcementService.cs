using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MVP_Core.Data;
using MVP_Core.Data.Models;
using MVP_Core.ViewModels.Compliance;
using Microsoft.EntityFrameworkCore;

namespace MVP_Core.Services.Compliance
{
    public class ComplianceEnforcementService
    {
        private readonly ApplicationDbContext _db;
        public ComplianceEnforcementService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<ExpiredDocument>> GetExpiredDocumentsAsync(Guid tenantId)
        {
            // TODO: Implement real expiration logic
            return new List<ExpiredDocument>
            {
                new ExpiredDocument { DocumentType = "Insurance", LastValidDate = DateTime.UtcNow.AddMonths(-2), FileStatus = "Expired", RequiresCertificateHolder = true },
                new ExpiredDocument { DocumentType = "License", LastValidDate = DateTime.UtcNow.AddMonths(-1), FileStatus = "Expired", RequiresCertificateHolder = false },
                new ExpiredDocument { DocumentType = "Certification", LastValidDate = DateTime.UtcNow.AddMonths(-3), FileStatus = "Expired", RequiresCertificateHolder = false }
            };
        }

        public async Task TriggerLockoutAsync(Guid tenantId, string reason)
        {
            // TODO: Implement lockout logic
        }

        public async Task LogUploadAsync(Guid tenantId, string documentType, string action, string? adminNote = null)
        {
            var log = new ComplianceOverrideLog
            {
                TenantId = tenantId,
                DocumentType = documentType,
                Action = action,
                Timestamp = DateTime.UtcNow,
                AdminNote = adminNote
            };
            _db.ComplianceOverrideLogs.Add(log);
            await _db.SaveChangesAsync();
        }
    }
}
