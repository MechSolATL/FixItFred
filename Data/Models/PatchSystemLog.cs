using System;
using System.ComponentModel.DataAnnotations;

namespace MVP_Core.Data.Models
{
    public class PatchSystemLog
    {
        public int Id { get; set; }

        [Required]
        public string Action { get; set; } = null!;

        [Required]
        public string PerformedBy { get; set; } = null!;

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public string? Notes { get; set; }
    }
}
