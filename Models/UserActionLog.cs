using System;

namespace MVP_Core.Models
{
    // Sprint 86.5 — User Action Log for Flow Compliance & Mentorship Analytics
    public class UserActionLog
    {
        public int Id { get; set; }
        public int AdminUserId { get; set; }
        public string ActionType { get; set; } = string.Empty; // e.g. "StepVisited", "ClickedSubmit", "SkippedStep"
        public string PageName { get; set; } = string.Empty;
        public string Route { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string SessionId { get; set; } = string.Empty;
        public bool IsError { get; set; }
        public string Detail { get; set; } = string.Empty;
    }
}
