// MVP_Core/Pages/Services/PlumbingQuestions.cshtml.cs
using Data;
using Data.Models;
using Data.Models.ViewModels;

namespace Pages.Services
{
    public class PlumbingQuestionsModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;

        public PlumbingQuestionsModel(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            Questions = [];
            UserAnswers = [];
        }

        public List<QuestionWithOptionsModel> Questions { get; set; }

        [BindProperty]
        public Dictionary<int, string> UserAnswers { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Load questions for Plumbing service
            Questions = await _dbContext.Questions
                .Where(static q => q.ServiceType == "Plumbing" && q.ParentQuestionId == null)
                .Include(static q => q.Options)
                .Select(static q => new QuestionWithOptionsModel
                {
                    Id = q.Id,
                    ServiceType = q.ServiceType,
                    Text = q.Text,
                    InputType = q.InputType,
                    IsMandatory = q.IsMandatory,
                    ParentQuestionId = q.ParentQuestionId,
                    ExpectedAnswer = q.ExpectedAnswer,
                    IsPrompt = q.IsPrompt,
                    PromptMessage = q.PromptMessage,
                    Page = q.Page,
                    Options = q.Options!.ToList()
                })
                .ToListAsync();

            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid || UserAnswers.Count == 0)
            {
                TempData["Error"] = "Please complete the form before submitting.";
                return RedirectToPage("/Services/PlumbingQuestions");
            }

            string sessionId = HttpContext.Session.Id;
            DateTime now = DateTime.UtcNow;

            foreach (KeyValuePair<int, string> answer in UserAnswers)
            {
                UserResponse userResponse = new()
                {
                    SessionID = sessionId,
                    QuestionId = answer.Key,
                    Response = answer.Value,
                    ServiceType = "Plumbing",
                    CreatedAt = now
                };
                _ = _dbContext.UserResponses.Add(userResponse);
            }

            _ = _dbContext.SaveChanges();

            return RedirectToPage("/Services/ThankYou", new { service = "Plumbing" });
        }
    }
}
