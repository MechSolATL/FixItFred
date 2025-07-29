// Sprint 90.1
using System;
using System.ComponentModel.DataAnnotations;

namespace MVP_Core.Data.Models
{
    // Sprint 90.1
    public class PromptVersion
    {
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;
        [Required, MaxLength(100)]
        public string Version { get; set; } = string.Empty;
        [Required]
        public string PromptText { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? CreatedBy { get; set; }
        public bool IsActive { get; set; } = true;
    }

    // Sprint 90.1
    public class PromptExperiment
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int PromptVersionId { get; set; }
        public PromptVersion PromptVersion { get; set; } = null!;
        [Required, MaxLength(100)]
        public string ExperimentName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime StartedAt { get; set; } = DateTime.UtcNow;
        public DateTime? EndedAt { get; set; }
        public string? CreatedBy { get; set; }
    }

    // Sprint 90.1
    public class PromptTraceLog
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int PromptVersionId { get; set; }
        public PromptVersion PromptVersion { get; set; } = null!;
        public int? ExperimentId { get; set; }
        public PromptExperiment? Experiment { get; set; }
        [Required]
        public string UserId { get; set; } = string.Empty;
        [Required]
        public string SessionId { get; set; } = string.Empty;
        [Required]
        public string Input { get; set; } = string.Empty;
        public string? Output { get; set; }
        public string? TraceJson { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    // Sprint 90.1
    public class LLMModelProvider
    {
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(100)]
        public string ProviderName { get; set; } = string.Empty;
        [Required, MaxLength(100)]
        public string ModelName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
