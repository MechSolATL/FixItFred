using System;

namespace ViewModels
{
    public class MetricsCardViewModel
    {
        public string Title { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string? Delta { get; set; }
        public string? RiskLevel { get; set; }
        public string? Module { get; set; }
        public string? LinkTo { get; set; }
    }
}
