// Sprint 91.17 - TroubleshootingBrain
using System;

namespace MVP_Core.Data.Models
{
    public class TroubleshootingAttemptLog
    {
        public int Id { get; set; }
        public Guid TechId { get; set; }
        public string PromptInput { get; set; }
        public string SuggestedFix { get; set; }
        public bool WasSuccessful { get; set; }
        public string TechNotes { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class KnownFix
    {
        public int Id { get; set; }
        public string ErrorCode { get; set; }
        public string EquipmentType { get; set; }
        public string Manufacturer { get; set; }
        public string CommonFix { get; set; }
        public int SuccessCount { get; set; }
        public DateTime LastConfirmed { get; set; }
    }
}
