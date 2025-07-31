namespace Data.Models
{
    public class QuestionModalViewModel
    {
        public required string ServiceType { get; set; } // Added 'required'
        public required Question CurrentQuestion { get; set; } // Added 'required'
        public required string CurrentAnswer { get; set; } // Added 'required'
        public int StepNumber { get; set; }

        public int TotalSteps { get; set; }
    }
}
