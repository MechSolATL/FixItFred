using System;

namespace MVP_Core.Data.Models
{
    public class SystemSnapshotLog
    {
        public int Id { get; set; }
        public string SnapshotType { get; set; } = string.Empty; // e.g. "FailureRecovery", "UserSession", "QuestionFlow"
        public string Summary { get; set; } = string.Empty;
        public string DetailsJson { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        public string SnapshotHash { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
