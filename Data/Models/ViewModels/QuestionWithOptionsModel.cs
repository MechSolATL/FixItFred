namespace Data.Models.ViewModels
{
    public class QuestionWithOptionsModel
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string GroupName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string ServiceType { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string Text { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string InputType { get; set; } = "text";

        public bool IsMandatory { get; set; }

        public int? ParentQuestionId { get; set; }

        [MaxLength(200)]
        public string? ExpectedAnswer { get; set; }

        public bool IsPrompt { get; set; }

        [MaxLength(500)]
        public string? PromptMessage { get; set; }

        [MaxLength(100)]
        public string? Page { get; set; }

        public List<QuestionOption> Options { get; set; } = [];
    }
}
