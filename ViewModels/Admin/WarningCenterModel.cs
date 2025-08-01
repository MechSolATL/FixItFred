using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MVP_Core.ViewModels.Admin
{
    // Sprint93_04 — Stub added by FixItFred
    public class WarningCenterModel
    {
        public int WarningId { get; set; }
        
        [Required]
        public string WarningMessage { get; set; }
        
        public string Severity { get; set; }
        
        public DateTime CreatedDate { get; set; }
        
        public bool IsAcknowledged { get; set; }
        
        public List<string> AffectedSystems { get; set; }
        
        // Sprint93_04 — Stub added by FixItFred
        public WarningCenterModel()
        {
            WarningMessage = string.Empty;
            Severity = "Low";
            CreatedDate = DateTime.UtcNow;
            IsAcknowledged = false;
            AffectedSystems = new List<string>();
        }
    }
}