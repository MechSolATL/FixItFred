// FixItFred – Sprint 44 Build Restoration
using System;

namespace MVP_Core.Models.Admin
{
    /// <summary>
    /// Represents an audit log entry for dispatcher actions.
    /// </summary>
    public class DispatcherAuditLog
    {
        public int Id { get; set; }
        public string ActionType { get; set; } = string.Empty;
        public int? RequestId { get; set; }
        public int? TechnicianId { get; set; }
        public string PerformedBy { get; set; } = string.Empty;
        public string PerformedByRole { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string? Notes { get; set; }
    }
}
