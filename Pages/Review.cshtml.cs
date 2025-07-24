using System.Text.Json;
using MVP_Core.Services;

namespace MVP_Core.Pages
{
    [ValidateAntiForgeryToken]
    public class ReviewModel : PageModel
    {
        private readonly QuestionService _questionService;
        private readonly ISeoService _seoService;
        private readonly IDeviceResolver _deviceResolver;
        private const string SessionAnswersKey = "Session.Modal.Answers";

        public List<QuestionAnswerPair> Answers { get; set; } = [];

        public ReviewModel(QuestionService questionService, ISeoService seoService, IDeviceResolver deviceResolver)
        {
            _questionService = questionService;
            _seoService = seoService;
            _deviceResolver = deviceResolver;
        }

        public async Task OnGetAsync()
        {
            var seo = await _seoService.GetSeoByPageNameAsync("Review");
            ViewData["Title"] = seo?.Title ?? "Review Your Answers";
            ViewData["MetaDescription"] = seo?.MetaDescription;
            ViewData["Keywords"] = seo?.Keywords;
            ViewData["Robots"] = seo?.Robots;
            ViewData["DeviceType"] = _deviceResolver.GetDeviceType(HttpContext);

            string? answerJson = HttpContext.Session.GetString(SessionAnswersKey);
            Dictionary<int, string> raw = string.IsNullOrEmpty(answerJson)
                ? []
                : JsonSerializer.Deserialize<Dictionary<int, string>>(answerJson) ?? [];

            foreach (KeyValuePair<int, string> entry in raw)
            {
                Question? question = _questionService.GetQuestionById(entry.Key);
                if (question != null)
                {
                    Answers.Add(new QuestionAnswerPair
                    {
                        QuestionId = question.Id,
                        QuestionText = question.Text,
                        AnswerText = entry.Value,
                        ServiceType = question.ServiceType
                    });
                }
            }
        }

        public IActionResult OnPostSubmit()
        {
            string? answerJson = HttpContext.Session.GetString(SessionAnswersKey);
            Dictionary<int, string> raw = string.IsNullOrEmpty(answerJson)
                ? []
                : JsonSerializer.Deserialize<Dictionary<int, string>>(answerJson) ?? [];

            if (!raw.Any())
            {
                TempData["ErrorMessage"] = "? No answers to save.";
                return RedirectToPage("/Review");
            }

            Question? firstQuestion = _questionService.GetQuestionById(raw.First().Key);
            string serviceType = firstQuestion?.ServiceType ?? "Unknown";

            foreach (KeyValuePair<int, string> entry in raw)
            {
                UserResponse response = new()
                {
                    SessionID = HttpContext.Session.Id,
                    QuestionId = entry.Key,
                    Response = entry.Value,
                    ServiceType = serviceType,
                    CreatedAt = DateTime.UtcNow
                };

                _questionService.AddUserResponse(response);
            }

            _questionService.SaveChanges();
            HttpContext.Session.Remove(SessionAnswersKey);

            TempData["SuccessMessage"] = "? Your service request has been submitted successfully!";
            return RedirectToPage("/Review");
        }

        public class QuestionAnswerPair
        {
            public int QuestionId { get; set; }
            public string QuestionText { get; set; } = "";
            public string AnswerText { get; set; } = "";
            public string? ServiceType { get; set; }
        }
    }
}
