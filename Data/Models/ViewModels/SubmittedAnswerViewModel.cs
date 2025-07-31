namespace Data.Models.ViewModels
{
    public class SubmittedAnswerViewModel
    {
        [Required(ErrorMessage = "Question text is required.")]
        [MaxLength(500, ErrorMessage = "Question text cannot exceed 500 characters.")]
        public string QuestionText { get; set; } = string.Empty;

        [Required(ErrorMessage = "Response is required.")]
        [MaxLength(2000, ErrorMessage = "Response cannot exceed 2000 characters.")]
        public string Response { get; set; } = string.Empty;
    }
}
