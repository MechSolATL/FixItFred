using System;
using System.ComponentModel.DataAnnotations;

namespace MVP_Core.Data.Models
{
    public class RecoveryLearningLog
    {
        public int Id { get; set; }
        public string TriggerSignature { get; set; } = string.Empty;
        public string PatternHash { get; set; } = string.Empty;
        public string Outcome { get; set; } = string.Empty;
        public DateTime RecordedAt { get; set; }
        public string SourceModule { get; set; } = string.Empty;
        [MaxLength(1000)]
        public string TriggerContextJson { get; set; } = string.Empty;
        public int? LinkedRequestId { get; set; }
    }
}
