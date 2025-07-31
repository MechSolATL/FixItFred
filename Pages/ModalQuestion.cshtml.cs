using Data.Models;
using Services;
using System.Text.Json;

namespace Pages
{
    public class ModalQuestionModel : PageModel
    {
        private readonly QuestionService _questionService;

        private const string SessionCurrentKey = "Session.Modal.CurrentQuestionId";
        private const string SessionAnswersKey = "Session.Modal.Answers";

        public ModalQuestionModel(QuestionService questionService)
        {
            _questionService = questionService;
        }

        [BindProperty]

        public QuestionModalViewModel ModalData { get; set; } = default!;


        public IActionResult OnGet(string service)
        {
            string? answerJson = HttpContext.Session.GetString(SessionAnswersKey);
            Dictionary<int, string> answerMap = string.IsNullOrEmpty(answerJson)
                ? []
                : JsonSerializer.Deserialize<Dictionary<int, string>>(answerJson) ?? [];

            int? currentId = HttpContext.Session.GetInt32(SessionCurrentKey);
            Question? question = currentId.HasValue
                ? _questionService.GetQuestionById(currentId.Value)
                : _questionService.GetFirstQuestion(service);

            if (question == null)
            {
                return Content("No questions found for selected service.");
            }

            ModalData = new QuestionModalViewModel
            {
                ServiceType = service,
                CurrentQuestion = question,
                CurrentAnswer = answerMap.TryGetValue(question.Id, out string? savedAnswer) ? savedAnswer : string.Empty,
                StepNumber = _questionService.GetStepIndex(service, question.Id),
                TotalSteps = _questionService.GetQuestionCount(service)
            };

            HttpContext.Session.SetInt32(SessionCurrentKey, question.Id);

            return Partial("_ServiceRequestModal", ModalData);
        }

        public QuestionModalViewModel GetModalData()
        {
            return ModalData;
        }

        public IActionResult OnPost(string service, QuestionModalViewModel modalData)
        {
            string? answerJson = HttpContext.Session.GetString(SessionAnswersKey);
            Dictionary<int, string> answers = string.IsNullOrEmpty(answerJson)
                ? []
                : JsonSerializer.Deserialize<Dictionary<int, string>>(answerJson) ?? [];

            answers[ModalData.CurrentQuestion.Id] = ModalData.CurrentAnswer ?? string.Empty;
            HttpContext.Session.SetString(SessionAnswersKey, JsonSerializer.Serialize(answers));

            Question? next = _questionService.GetNextQuestion(service,
                                                              ModalData.CurrentQuestion.Id,
                                                              currentAnswer: modalData.CurrentAnswer);
            if (next == null)
            {
                return RedirectToPage("/Review");
            }

            HttpContext.Session.SetInt32(SessionCurrentKey, next.Id);

            ModalData.ServiceType = service;
            ModalData.CurrentQuestion = next;
            ModalData.CurrentAnswer = answers.TryGetValue(next.Id, out string? savedAnswer) ? savedAnswer : string.Empty;
            ModalData.StepNumber = _questionService.GetStepIndex(service, next.Id);
            ModalData.TotalSteps = _questionService.GetQuestionCount(service);

            return Partial("_ServiceRequestModal", ModalData);
        }

        public IActionResult OnPostBack(string service)
        {
            Question? prev = _questionService.GetPreviousQuestion(service, ModalData.CurrentQuestion.Id);
            if (prev == null)
            {
                return RedirectToPage();
            }

            string? answerJson = HttpContext.Session.GetString(SessionAnswersKey);
            Dictionary<int, string> answers = string.IsNullOrEmpty(answerJson)
                ? []
                : JsonSerializer.Deserialize<Dictionary<int, string>>(answerJson) ?? [];

            HttpContext.Session.SetInt32(SessionCurrentKey, prev.Id);

            ModalData.ServiceType = service;
            ModalData.CurrentQuestion = prev;
            ModalData.CurrentAnswer = answers.TryGetValue(prev.Id, out string? savedAnswer) ? savedAnswer : string.Empty;
            ModalData.StepNumber = _questionService.GetStepIndex(service, prev.Id);
            ModalData.TotalSteps = _questionService.GetQuestionCount(service);

            return Partial("_ServiceRequestModal", ModalData);
        }

        public IActionResult OnPostReset()
        {
            HttpContext.Session.Remove(SessionCurrentKey);
            HttpContext.Session.Remove(SessionAnswersKey);
            return RedirectToPage(); // Restart flow
        }
    }
}
