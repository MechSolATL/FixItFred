using System;

namespace MVP_Core.Data.Models
{
    public class RecoveryLearningLog
    {
        public int Id { get; set; }
        public string TriggerSignature { get; set; } = string.Empty;
        public string PatternHash { get; set; } = string.Empty;
        public string Outcome { get; set; } = string.Empty;
        public DateTime RecordedAt { get; set; }
    }
}
