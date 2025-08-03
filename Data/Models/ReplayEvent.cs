using System.ComponentModel.DataAnnotations;

namespace Data.Models
{
    /// <summary>
    /// [Sprint126_OneScan_01-20] BanterReplayEvent model for tracking banter escalation and other replay events
    /// Used for auditing and replay functionality across Patch & Lyra systems
    /// </summary>
    public class BanterReplayEvent
    {
        [Key]
        public Guid Id { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string EmployeeId { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(100)]
        public string EventType { get; set; } = string.Empty;
        
        [Required]
        public string EventData { get; set; } = string.Empty;
        
        [Required]
        public DateTime Timestamp { get; set; }
        
        [MaxLength(500)]
        public string Context { get; set; } = string.Empty;
        
        [MaxLength(20)]
        public string Severity { get; set; } = "Info";
        
        // Navigation properties for potential relationships
        [MaxLength(100)]
        public string? TriggerSource { get; set; }
        
        public bool IsProcessed { get; set; } = false;
        
        public DateTime? ProcessedAt { get; set; }
        
        [MaxLength(100)]
        public string? ProcessedBy { get; set; }
    }
}