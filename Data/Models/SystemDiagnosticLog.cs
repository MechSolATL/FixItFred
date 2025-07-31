using System;
using System.ComponentModel.DataAnnotations;

namespace Data.Models
{
    public enum DiagnosticStatusLevel
    {
        OK,
        Warning,
        Error
    }

    public class SystemDiagnosticLog
    {
        [Key]
        public int Id { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        [MaxLength(100)]
        public string ModuleName { get; set; } = string.Empty;
        public DiagnosticStatusLevel StatusLevel { get; set; } = DiagnosticStatusLevel.OK;
        [MaxLength(200)]
        public string Summary { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
    }
}
