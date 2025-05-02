using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Data;
using MVP_Core.Data.Models;
using MVP_Core.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;
using System;
using MVP_Core.Models;
using MVP_Core.Services;

namespace MVP_Core.Pages.Services
{
    public class AirConditioningModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ISeoService _seoService;

        public AirConditioningModel(ApplicationDbContext dbContext, ISeoService seoService)
        {
            _dbContext = dbContext;
            _seoService = seoService;
        }

        public Question? CurrentQuestion { get; set; }

        [BindProperty] public string UserAnswer { get; set; } = string.Empty;
        [BindProperty] public string Message { get; set; } = string.Empty;
        [BindProperty] public bool IsSuccess { get; set; }

        private const string SessionKey = "ServiceRequest";
        private const string SessionStartKey = "ServiceRequestStart";

        public int CurrentStep { get; set; }
        public int TotalQuestions { get; set; }
        public string ProgressPercent => TotalQuestions > 0 ? $"{CurrentStep * 100 / TotalQuestions}%" : "0%";

        public string? FirstName => HttpContext.Session.GetObject<ServiceRequestSession>(SessionKey)?.CustomerName?.Split(' ').FirstOrDefault();
        public bool IsVerified { get; set; }
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

            // Inject SEO from database
            var seo = await _seoService.GetSeoByPageNameAsync("AirConditioning");
            ViewData["Title"] = seo?.Title ?? "Air Conditioning Service Request";
            ViewData["MetaDescription"] = seo?.MetaDescription ?? "Request expert air conditioning service from Service-Atlanta.";
            ViewData["Keywords"] = seo?.Keywords ?? "HVAC, air conditioning, cooling, AC repair, Service-Atlanta";
            ViewData["Robots"] = seo?.RobotsMeta ?? "index, follow";

            var session = HttpContext.Session.GetObject<ServiceRequestSession>(SessionKey) ?? new ServiceRequestSession { ServiceType = "AirConditioning" };
            HttpContext.Session.SetObject(SessionKey, session);

            if (string.IsNullOrEmpty(HttpContext.Session.GetString(SessionStartKey)))
                HttpContext.Session.SetString(SessionStartKey, DateTime.UtcNow.ToString());

            if (IsSessionExpired())
            {
                TempData["Expired"] = true;
                HttpContext.Session.Clear();
                return RedirectToPage("/Services/AirConditioning");
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
                .Where(q => q.ServiceType == "AirConditioning")
                .Where(q => q.ParentQuestionId == null ||
                    (session.Answers.ContainsKey(q.ParentQuestionId.Value) && session.Answers[q.ParentQuestionId.Value].Answer == q.ExpectedAnswer))
                .Where(q => !answeredIds.Contains(q.Id))
                .OrderBy(q => q.Id)
                .FirstOrDefaultAsync();

            if (CurrentQuestion == null)
            {
                return RedirectToPage("/Services/AirConditioningFinal");
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
                return RedirectToPage("/Services/AirConditioning");
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
            else if (Request.Form.ContainsKey("questionId") && int.TryParse(Request.Form["questionId"], out var questionId))
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
                return RedirectToPage("/Services/AirConditioning");
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
                .Where(q => q.ServiceType == "AirConditioning")
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
