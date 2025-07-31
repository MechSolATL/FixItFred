using System.ComponentModel.DataAnnotations;

namespace Data.Models
{
    public class Question
    {
        public int Id { get; set; }

        /// <summary>
        /// Logical grouping for admin filtering (e.g., "Install Flow", "Diagnostics").
        /// </summary>
        public string GroupName { get; set; } = string.Empty;

        /// <summary>
        /// Which service this question applies to (Plumbing, Heating, etc). Use "Global" for reusable ones.
        /// </summary>
        [Required]
        [StringLength(100)]
        public string ServiceType { get; set; } = string.Empty;

        /// <summary>
        /// The actual question shown to the user.
        /// </summary>
        [Required]
        [StringLength(200)]
        public string Text { get; set; } = string.Empty;

        /// <summary>
        /// Input type expected ("text", "textarea", "dropdown").
        /// </summary>
        [Required]
        public string InputType { get; set; } = string.Empty;

        /// <summary>
        /// Whether this question is mandatory for form submission.
        /// </summary>
        public bool IsMandatory { get; set; } = false;

        /// <summary>
        /// Optional reference to a parent question that triggers this one.
        /// </summary>
        public int? ParentQuestionId { get; set; }

        /// <summary>
        /// The expected answer from parent that triggers this question.
        /// </summary>
        public string? ExpectedAnswer { get; set; }

        /// <summary>
        /// If true, shows this question as a prompt instead of full step.
        /// </summary>
        public bool IsPrompt { get; set; } = false;

        /// <summary>
        /// Optional prompt message to display when question is marked as prompt.
        /// </summary>
        public string? PromptMessage { get; set; }

        /// <summary>
        /// Page or flow this question belongs to (e.g. "PlumbingRequest").
        /// </summary>
        public string? Page { get; set; }

        /// <summary>
        /// If true, this question is global and reused across services.
        /// </summary>
        /// [NotMapped] // EF will skip this property during migrations
        public bool IsGlobal { get; set; } = false;

        /// <summary>
        /// Optional navigation to dropdown-style options for this question.
        /// </summary>
        public ICollection<QuestionOption>? Options { get; set; }
    }
}
