using System;
using System.ComponentModel.DataAnnotations;

namespace MVP_Core.ViewModels.Admin
{
    // Sprint93_04 — Stub added by FixItFred
    public class TrustRebuildModel
    {
        [Required]
        public string SystemId { get; set; }
        
        public DateTime LastRebuildDate { get; set; }
        
        public bool IsRebuildRequired { get; set; }
        
        public string RebuildStatus { get; set; }
        
        // Sprint93_04 — Stub added by FixItFred
        public TrustRebuildModel()
        {
            SystemId = string.Empty;
            RebuildStatus = "Pending";
            LastRebuildDate = DateTime.MinValue;
            IsRebuildRequired = false;
        }
    }
}