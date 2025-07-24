using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Data;
using MVP_Core.Data.Models;
using System.Threading.Tasks;

namespace MVP_Core.Pages.Admin
{
    public class EditQuestionModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;
        public EditQuestionModel(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [BindProperty]
        public Question Question { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id.HasValue)
            {
                var found = await _dbContext.Questions.FindAsync(id.Value);
                if (found == null)
                {
                    TempData["Error"] = "Question not found.";
                    return RedirectToPage("/Admin/ManageQuestions");
                }
                Question = found;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            if (Question.Id > 0)
            {
                var existing = await _dbContext.Questions.FindAsync(Question.Id);
                if (existing == null)
                {
                    TempData["Error"] = "Question not found.";
                    return RedirectToPage("/Admin/ManageQuestions");
                }
                existing.ServiceType = Question.ServiceType;
                existing.Text = Question.Text;
                existing.InputType = Question.InputType;
                existing.ParentQuestionId = Question.ParentQuestionId;
                existing.ExpectedAnswer = Question.ExpectedAnswer;
                existing.IsMandatory = Question.IsMandatory;
                existing.IsPrompt = Question.IsPrompt;
                existing.PromptMessage = Question.PromptMessage;
                existing.Page = Question.Page;
            }
            else
            {
                _dbContext.Questions.Add(Question);
            }
            await _dbContext.SaveChangesAsync();
            TempData["Message"] = "? Question saved.";
            return RedirectToPage("/Admin/ManageQuestions");
        }
    }
}
