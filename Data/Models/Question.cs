namespace MVP_Core.Data.Models
{
    public class Question
    {
        public int Id { get; set; }
        public string GroupName { get; set; } = string.Empty;  // Logical grouping (e.g., Repair, General Info)
        public string ServiceType { get; set; } = string.Empty; // Repair, Warranty, etc.
        public string QuestionText { get; set; } = string.Empty; // The actual question
        public string InputType { get; set; } = "text"; // text, checkbox, dropdown
        public bool IsMandatory { get; set; } = false; // Whether it's required
        public int? ParentQuestionId { get; set; } // For conditional rendering
        public string? ExpectedAnswer { get; set; } // For validation purposes
        public bool IsPrompt { get; set; } = false; // Whether to display a prompt
        public string? PromptMessage { get; set; } // Optional guidance for users
        public string? Page { get; set; } // Optional: To filter questions by page

        // **New Property** for dropdown options
        public List<string>? Options { get; set; }
    }
}
