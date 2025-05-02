// MVP_Core/Pages/Services/PlumbingQuestions.cshtml.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Data;
using MVP_Core.Data.Models;
using MVP_Core.Data.Models.ViewModels;
using MVP_Core.Services;
using Microsoft.EntityFrameworkCore;

namespace MVP_Core.Pages.Services
{
    public class PlumbingQuestionsModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;

        public PlumbingQuestionsModel(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            Questions = new List<QuestionWithOptionsModel>();
            UserAnswers = new Dictionary<int, string>();
        }

        public List<QuestionWithOptionsModel> Questions { get; set; }

        [BindProperty]
        public Dictionary<int, string> UserAnswers { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Load questions for Plumbing service
            Questions = await _dbContext.Questions
                .Where(q => q.ServiceType == "Plumbing" && q.ParentQuestionId == null)
                .Include(q => q.Options)
                .Select(q => new QuestionWithOptionsModel
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

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid || UserAnswers.Count == 0)
            {
                TempData["Error"] = "Please complete the form before submitting.";
                return RedirectToPage("/Services/PlumbingQuestions");
            }

            var sessionId = HttpContext.Session.Id;
            var now = DateTime.UtcNow;

            foreach (var answer in UserAnswers)
            {
                var userResponse = new UserResponse
                {
                    SessionID = sessionId,
                    QuestionId = answer.Key,
                    Response = answer.Value,
                    ServiceType = "Plumbing",
                    CreatedAt = now
                };
                _dbContext.UserResponses.Add(userResponse);
            }

            await _dbContext.SaveChangesAsync();

            return RedirectToPage("/Services/ThankYou", new { service = "Plumbing" });
        }
    }
}