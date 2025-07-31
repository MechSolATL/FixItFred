// Sprint 90.1
using System;
using System.ComponentModel.DataAnnotations;

namespace Data.Models
{
    /// <summary>
    /// Represents a version of a prompt used in the application.
    /// </summary>
    public class PromptVersion
    {
        /// <summary>
        /// Gets or sets the unique identifier for the prompt version.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the prompt version.
        /// </summary>
        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the version string of the prompt.
        /// </summary>
        [Required, MaxLength(100)]
        public string Version { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the text of the prompt.
        /// </summary>
        [Required]
        public string PromptText { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the creation timestamp of the prompt version.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the user who created the prompt version.
        /// </summary>
        public string? CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the prompt version is active.
        /// </summary>
        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// Represents an experiment conducted with a specific prompt version.
    /// </summary>
    public class PromptExperiment
    {
        /// <summary>
        /// Gets or sets the unique identifier for the prompt experiment.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the associated prompt version.
        /// </summary>
        [Required]
        public int PromptVersionId { get; set; }

        /// <summary>
        /// Gets or sets the associated prompt version.
        /// </summary>
        public PromptVersion PromptVersion { get; set; } = null!;

        /// <summary>
        /// Gets or sets the name of the experiment.
        /// </summary>
        [Required, MaxLength(100)]
        public string ExperimentName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description of the experiment.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the start timestamp of the experiment.
        /// </summary>
        public DateTime StartedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the end timestamp of the experiment.
        /// </summary>
        public DateTime? EndedAt { get; set; }

        /// <summary>
        /// Gets or sets the user who created the experiment.
        /// </summary>
        public string? CreatedBy { get; set; }
    }

    /// <summary>
    /// Represents a provider for LLM models.
    /// </summary>
    public class LLMModelProvider
    {
        /// <summary>
        /// Gets or sets the unique identifier for the LLM model provider.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the provider.
        /// </summary>
        [Required, MaxLength(100)]
        public string ProviderName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the name of the model provided.
        /// </summary>
        [Required, MaxLength(100)]
        public string ModelName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description of the model provider.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the model provider is active.
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
}
