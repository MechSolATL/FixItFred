using MVP_Core.Data;
using MVP_Core.Data.Models;
using MVP_Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;

namespace MVP_Core.Pages.Admin
{
    public class SurveyBuilderModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly SurveyService _surveyService;
        public SurveyBuilderModel(ApplicationDbContext db)
        {
            _db = db;
            _surveyService = new SurveyService(db);
        }
        [BindProperty]
        public string ServiceType { get; set; } = string.Empty;
        [BindProperty]
        public string QuestionText { get; set; } = string.Empty;
        [BindProperty]
        public string InputType { get; set; } = "slider";
        [BindProperty]
        public int SortOrder { get; set; }
        public Dictionary<string, List<FeedbackSurveyTemplate>> QuestionsByServiceType { get; set; } = new();
        public void OnGet()
        {
            QuestionsByServiceType = _db.FeedbackSurveyTemplates
                .GroupBy(q => q.ServiceType)
                .ToDictionary(g => g.Key, g => g.OrderBy(q => q.SortOrder).ToList());
        }
        public IActionResult OnPost()
        {
            if (!string.IsNullOrWhiteSpace(ServiceType) && !string.IsNullOrWhiteSpace(QuestionText))
            {
                var question = new FeedbackSurveyTemplate
                {
                    ServiceType = ServiceType,
                    QuestionText = QuestionText,
                    InputType = InputType,
                    SortOrder = SortOrder
                };
                _db.FeedbackSurveyTemplates.Add(question);
                _db.SaveChanges();
            }
            return RedirectToPage();
        }
        public IActionResult OnPostDelete(int id)
        {
            var question = _db.FeedbackSurveyTemplates.FirstOrDefault(q => q.Id == id);
            if (question != null)
            {
                _db.FeedbackSurveyTemplates.Remove(question);
                _db.SaveChanges();
            }
            return RedirectToPage();
        }
    }
}
