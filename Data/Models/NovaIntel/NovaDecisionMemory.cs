using System;

namespace MVP_Core.Data.Models.NovaIntel
{
    public class NovaDecisionMemory
    {
        public int Id { get; set; }
        public Guid TenantId { get; set; }
        public string DecisionType { get; set; } // e.g., "OverrideGranted", "LockoutEnforced", etc.
        public string ReasonProvided { get; set; }
        public string InitiatedBy { get; set; } // admin or AI
        public DateTime Timestamp { get; set; }
        public string LinkedObjectType { get; set; } // "Invoice", "Compliance", "Badge"
        public string LinkedObjectId { get; set; } // e.g., InvoiceId or ComplianceId
    }
}
