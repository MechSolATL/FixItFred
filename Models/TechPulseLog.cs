using System;
using System.ComponentModel.DataAnnotations;

namespace MVP_Core.Models
{
    public record TechPulseLog
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Emoji { get; set; } = string.Empty;
        public DateTime Date { get; set; } = DateTime.UtcNow.Date;
    }
}
