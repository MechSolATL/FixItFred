using Data;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pages.Admin
{
    public class ManageOptionsModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;
        public ManageOptionsModel(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [BindProperty(SupportsGet = true)]
        public int QuestionId { get; set; }
        public List<QuestionOption> Options { get; set; } = new();
        [BindProperty]
        public QuestionOption NewOption { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int questionId)
        {
            QuestionId = questionId;
            Options = await _dbContext.QuestionOptions
                .Where(o => o.QuestionId == questionId)
                .OrderBy(o => o.Id)
                .ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAddAsync()
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(NewOption.OptionText))
            {
                TempData["Error"] = "Option text is required.";
                return await OnGetAsync(QuestionId);
            }
            NewOption.QuestionId = QuestionId;
            _dbContext.QuestionOptions.Add(NewOption);
            await _dbContext.SaveChangesAsync();
            TempData["Message"] = "Option added.";
            return RedirectToPage(new { questionId = QuestionId });
        }

        public async Task<IActionResult> OnPostDeleteAsync(int optionId)
        {
            var opt = await _dbContext.QuestionOptions.FindAsync(optionId);
            if (opt != null)
            {
                _dbContext.QuestionOptions.Remove(opt);
                await _dbContext.SaveChangesAsync();
                TempData["Message"] = "Option deleted.";
            }
            else
            {
                TempData["Error"] = "Option not found.";
            }
            return RedirectToPage(new { questionId = QuestionId });
        }
    }
}
