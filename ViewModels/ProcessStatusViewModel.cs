using System;

namespace MVP_Core.ViewModels
{
    public enum ProcessHealthStatus
    {
        Unknown = 0,
        Healthy = 1,
        Warning = 2,
        Critical = 3
    }

    public class ProcessStatusViewModel
    {
        public string Name { get; set; } = string.Empty;
        public ProcessHealthStatus Status { get; set; } = ProcessHealthStatus.Unknown;
        public DateTime LastChecked { get; set; }
        public string SuggestedAction { get; set; } = string.Empty;
    }
}
