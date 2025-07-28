using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace MVP_Core.ViewModels.Compliance
{
    public class ExpiredDocument
    {
        public string DocumentType { get; set; } = string.Empty;
        public DateTime? LastValidDate { get; set; }
        public string FileStatus { get; set; } = string.Empty;
        public bool RequiresCertificateHolder { get; set; }
    }

    public class ComplianceRecoveryViewModel
    {
        public List<ExpiredDocument> ExpiredDocuments { get; set; } = new();
        public Dictionary<string, IFormFile?> Uploads { get; set; } = new();
        public string? HardshipNote { get; set; }
    }
}
