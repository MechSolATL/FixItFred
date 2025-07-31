using System;

namespace Models.ViewModels
{
    public class DiagnosticsAlertResult
    {
        public string Type { get; set; } = string.Empty;
        public string SeverityLevel { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string SuggestedFix { get; set; } = string.Empty;
    }
}
