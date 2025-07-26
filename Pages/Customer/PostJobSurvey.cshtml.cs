using MVP_Core.Data;
using MVP_Core.Data.Models;
using MVP_Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;

namespace MVP_Core.Pages.Customer
{
    public class PostJobSurveyModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly SurveyService _surveyService;
        public PostJobSurveyModel(ApplicationDbContext db)
        {
            _db = db;
            _surveyService = new SurveyService(db);
        }
        [BindProperty]
        public int ServiceRequestId { get; set; }
        public List<FeedbackSurveyTemplate> SurveyQuestions { get; set; } = new();
        public bool Submitted { get; set; } = false;
        public void OnGet(int serviceRequestId)
        {
            ServiceRequestId = serviceRequestId;
            var request = _db.ServiceRequests.FirstOrDefault(r => r.Id == serviceRequestId);
            if (request != null)
            {
                SurveyQuestions = _surveyService.GetSurveyByServiceType(request.ServiceType);
            }
        }
        public IActionResult OnPost(Dictionary<int, string> responses)
        {
            var request = _db.ServiceRequests.FirstOrDefault(r => r.Id == ServiceRequestId);
            if (request == null)
                return Page();
            var surveyQuestions = _surveyService.GetSurveyByServiceType(request.ServiceType);
            var feedbackResponses = new List<FeedbackResponse>();
            foreach (var q in surveyQuestions)
            {
                if (responses.TryGetValue(q.Id, out var value))
                {
                    feedbackResponses.Add(new FeedbackResponse
                    {
                        SurveyTemplateId = q.Id,
                        CustomerEmail = request.Email,
                        ResponseValue = value,
                        Timestamp = DateTime.UtcNow,
                        ServiceRequestId = request.Id
                    });
                }
            }
            _surveyService.SaveResponses(feedbackResponses);
            Submitted = true;
            return Page();
        }
    }
}
