using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace MVP_Core.Models
{
    // Sprint 85.9 — Community Gamification Models
    public record QuoteMessage
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Message { get; set; } = string.Empty;
        [Required]
        public string Author { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public record Whisper
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Sender { get; set; } = string.Empty;
        [Required]
        public string Recipient { get; set; } = string.Empty;
        [Required]
        public string Message { get; set; } = string.Empty;
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
    }

    public record LeaderboardEntry
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public int Points { get; set; }
        public int Rank { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
        [NotMapped]
        public List<string> Badges { get; set; } = new();
        // For EF Core storage of badges as JSON
        public string BadgesJson
        {
            get => JsonSerializer.Serialize(Badges);
            set => Badges = string.IsNullOrWhiteSpace(value) ? new List<string>() : JsonSerializer.Deserialize<List<string>>(value) ?? new List<string>();
        }
    }
}
