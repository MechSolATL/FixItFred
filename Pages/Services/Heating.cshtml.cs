using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Data;
using MVP_Core.Data.Models;
using MVP_Core.Helpers;
using Microsoft.EntityFrameworkCore;
using MVP_Core.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MVP_Core.Pages.Services
{
    public class HeatingModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;

        public HeatingModel(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Question? CurrentQuestion { get; set; }

        [BindProperty]
        public string UserAnswer { get; set; } = string.Empty;

        [BindProperty]
        public string Message { get; set; } = string.Empty;

        [BindProperty]
        public bool IsSuccess { get; set; } = false;

        private const string SessionKey = "ServiceRequest";
        private const string SessionStartKey = "ServiceRequestStart";

        public int CurrentStep { get; set; } = 0;
        public int TotalQuestions { get; set; } = 0;
        public string ProgressPercent => TotalQuestions > 0 ? $"{CurrentStep * 100 / TotalQuestions}%" : "0%";

        public string? FirstName => HttpContext.Session.GetObject<ServiceRequestSession>(SessionKey)?.CustomerName?.Split(' ').FirstOrDefault();
        public bool IsVerified { get; set; } = false;
        public string VerifiedEmail { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync()
        {
            IsVerified = HttpContext.Session.GetString("IsEmailVerified") == "true";
            VerifiedEmail = HttpContext.Session.GetString("VerifiedEmail") ?? string.Empty;

            if (!IsVerified)
            {
                Message = "Please verify your email before proceeding.";
                return Page();
            }

            var session = HttpContext.Session.GetObject<ServiceRequestSession>(SessionKey)
                          ?? new ServiceRequestSession { ServiceType = "Heating" };

            HttpContext.Session.SetObject(SessionKey, session);

            if (IsSessionExpired())
            {
                TempData["Expired"] = true;
                HttpContext.Session.Clear();
                return RedirectToPage("/Services/Heating");
            }

            var answeredIds = session.Answers.Keys.ToList();

            if (string.IsNullOrWhiteSpace(session.CustomerName))
            {
                CurrentQuestion = new Question { Text = "👋 Hi there! What is your full name?", InputType = "text" };
                SetProgress(1, 2 + await GetTotalDynamicQuestions());
                return Page();
            }

            if (string.IsNullOrWhiteSpace(session.Address))
            {
                CurrentQuestion = new Question { Text = $"🏡 Nice to meet you {FirstName}! What's the service address?", InputType = "text" };
                SetProgress(2, 2 + await GetTotalDynamicQuestions());
                return Page();
            }

            CurrentQuestion = await _dbContext.Questions
                .Include(q => q.Options)
                .Where(q => q.ServiceType == "Heating")
                .Where(q => q.ParentQuestionId == null ||
                    (session.Answers.ContainsKey(q.ParentQuestionId.Value) &&
                     session.Answers[q.ParentQuestionId.Value].Answer == q.ExpectedAnswer))
                .Where(q => !answeredIds.Contains(q.Id))
                .OrderBy(q => q.Id)
                .FirstOrDefaultAsync();

            if (CurrentQuestion == null)
            {
                return RedirectToPage("/Services/HeatingFinal");
            }

            SetProgress(2 + answeredIds.Count, 2 + await GetTotalDynamicQuestions());
            return Page();
        }

        public async Task<IActionResult> OnPostNextAsync()
        {
            var session = HttpContext.Session.GetObject<ServiceRequestSession>(SessionKey);

            if (session == null)
            {
                TempData["Expired"] = true;
                return RedirectToPage("/Services/Heating");
            }

            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(UserAnswer) || UserAnswer.Trim().Length > 500)
            {
                Message = "⚠️ Please provide a valid answer (max 500 characters).";
                return await OnGetAsync();
            }

            var cleanAnswer = UserAnswer.Trim();

            if (string.IsNullOrWhiteSpace(session.CustomerName))
            {
                session.CustomerName = cleanAnswer;
            }
            else if (string.IsNullOrWhiteSpace(session.Address))
            {
                session.Address = cleanAnswer;
            }
            else if (Request.Form.TryGetValue("questionId", out var qid) && int.TryParse(qid, out var questionId))
            {
                session.Answers[questionId] = (cleanAnswer, DateTime.UtcNow);
            }

            HttpContext.Session.SetObject(SessionKey, session);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostBackAsync()
        {
            var session = HttpContext.Session.GetObject<ServiceRequestSession>(SessionKey);

            if (session == null)
            {
                TempData["Expired"] = true;
                return RedirectToPage("/Services/Heating");
            }

            if (session.Answers.Any())
            {
                var lastKey = session.Answers.Keys.Last();
                session.Answers.Remove(lastKey);
            }
            else if (!string.IsNullOrWhiteSpace(session.Address))
            {
                session.Address = string.Empty;
            }
            else if (!string.IsNullOrWhiteSpace(session.CustomerName))
            {
                session.CustomerName = string.Empty;
            }

            HttpContext.Session.SetObject(SessionKey, session);
            return RedirectToPage();
        }

        private void SetProgress(int currentStep, int totalSteps)
        {
            CurrentStep = currentStep;
            TotalQuestions = totalSteps;
        }

        private async Task<int> GetTotalDynamicQuestions()
        {
            return await _dbContext.Questions
                .Where(q => q.ServiceType == "Heating")
                .CountAsync();
        }

        private bool IsSessionExpired()
        {
            var sessionStartString = HttpContext.Session.GetString(SessionStartKey);
            if (!DateTime.TryParse(sessionStartString, out var sessionStart))
                return true;

            return (DateTime.UtcNow - sessionStart).TotalMinutes > 20;
        }
    }
}
