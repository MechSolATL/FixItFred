using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Helpers;
using Services;
using Data;
using Data.Models;

namespace Pages.Services
{
    [ValidateAntiForgeryToken]
    // Sprint 26.5 Patch Log: Dynamic form flow cloned from Plumbing for Air Conditioning. Multi-step, progress, session, SEO metadata.
    public class AirConditioningModel : PageModel
    {
        private readonly QuestionService _questionService;
        private readonly ISeoService _seoService;
        private readonly ApplicationDbContext _dbContext;
        private readonly SessionTracker _session;
        private readonly IDeviceResolver _deviceResolver;

        public AirConditioningModel(QuestionService questionService, ISeoService seoService, ApplicationDbContext dbContext, SessionTracker session, IDeviceResolver deviceResolver)
        {
            _questionService = questionService;
            _seoService = seoService;
            _dbContext = dbContext;
            _session = session;
            _deviceResolver = deviceResolver;
        }

        public Question? CurrentQuestion { get; set; }
        [BindProperty] public string UserAnswer { get; set; } = string.Empty;
        [BindProperty] public string Message { get; set; } = string.Empty;
        [BindProperty] public bool IsSuccess { get; set; }
        public List<string> CarouselImages { get; set; } = new();
        public string? MarketingText { get; set; }
        public int CurrentStep { get; set; }
        public int TotalQuestions { get; set; }
        public string ProgressPercent => TotalQuestions > 0 ? $"{CurrentStep * 100 / TotalQuestions}%" : "0%";
        public string? FirstName => _session.CustomerName?.Split(' ').FirstOrDefault();
        public bool IsVerified => _session.IsVerified;
        public string VerifiedEmail => _session.CustomerEmail;

        private const string SessionKey = "ServiceRequestAirConditioning";
        private const string SessionStartKey = "ServiceRequestAirConditioningStart";

        public async Task<IActionResult> OnGetAsync()
        {
            if (!IsVerified)
            {
                Message = "Please verify your email before proceeding.";
                return Page();
            }

            // SEO
            var seo = await _seoService.GetSeoMetaAsync("Services/AirConditioning");
            ViewData["Title"] = seo?.Title ?? "Air Conditioning Service Request";
            ViewData["MetaDescription"] = seo?.MetaDescription ?? "";
            ViewData["Keywords"] = seo?.Keywords ?? "";
            ViewData["Robots"] = seo?.Robots ?? "index, follow";
            ViewData["DeviceType"] = _deviceResolver.GetDeviceType(HttpContext);

            // Content
            CarouselImages = await _dbContext.Contents
                .Where(c => c.PageName == "Services/AirConditioning" && c.Section.StartsWith("Carousel"))
                .OrderBy(c => c.Section)
                .Select(c => c.ContentText)
                .ToListAsync();
            MarketingText = await _dbContext.Contents
                .Where(c => c.PageName == "Services/AirConditioning" && c.Section == "MarketingText")
                .Select(c => c.ContentText)
                .FirstOrDefaultAsync();

            // Session Setup
            var session = HttpContext.Session.GetObject<ServiceRequestSession>(SessionKey)
                ?? new ServiceRequestSession { ServiceType = "Air Conditioning" };
            HttpContext.Session.SetObject(SessionKey, session);

            if (IsSessionExpired())
            {
                TempData["Expired"] = true;
                _session.ClearAll();
                return RedirectToPage("/Services/AirConditioning");
            }

            var answeredIds = session.Answers?.Keys.ToList() ?? new List<int>();

            if (string.IsNullOrWhiteSpace(session.CustomerName))
            {
                CurrentQuestion = new Question
                {
                    Id = -1,
                    Text = "What is your full name?",
                    InputType = "text"
                };
                SetProgress(1, 2 + await GetTotalDynamicQuestions());
                return Page();
            }

            if (string.IsNullOrWhiteSpace(session.Address))
            {
                CurrentQuestion = new Question
                {
                    Id = -2,
                    Text = $"Nice to meet you {FirstName}! What's the service address?",
                    InputType = "text"
                };
                SetProgress(2, 2 + await GetTotalDynamicQuestions());
                return Page();
            }

            CurrentQuestion = await _dbContext.Questions
                .Include(q => q.Options)
                .Where(q => q.ServiceType == "Air Conditioning")
                .Where(q => q.ParentQuestionId == null ||
                            session.Answers != null && session.Answers.ContainsKey(q.ParentQuestionId.Value) &&
                             session.Answers[q.ParentQuestionId.Value].Answer == q.ExpectedAnswer)
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
                Message = "Please provide a valid answer (max 500 characters).";
                return await OnGetAsync();
            }

            string cleanAnswer = UserAnswer.Trim();
            if (Request.Form.ContainsKey("questionId") && int.TryParse(Request.Form["questionId"], out int qid))
            {
                if (qid == -1)
                {
                    session.CustomerName = cleanAnswer;
                }
                else if (qid == -2)
                {
                    session.Address = cleanAnswer;
                }
                else
                {
                    session.Answers[qid] = (cleanAnswer, DateTime.UtcNow);
                }
            }

            HttpContext.Session.SetObject(SessionKey, session);
            return RedirectToPage();
        }

        public IActionResult OnPostBack()
        {
            var session = HttpContext.Session.GetObject<ServiceRequestSession>(SessionKey);
            if (session == null)
            {
                TempData["Expired"] = true;
                return RedirectToPage("/Services/AirConditioning");
            }

            if (session.Answers.Any())
            {
                _ = session.Answers.Remove(session.Answers.Keys.Last());
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
            return await _dbContext.Questions.CountAsync(q => q.ServiceType == "Air Conditioning");
        }

        private bool IsSessionExpired()
        {
            string? sessionStartString = HttpContext.Session.GetString(SessionStartKey);
            return !DateTime.TryParse(sessionStartString, out DateTime sessionStart) || (DateTime.UtcNow - sessionStart).TotalMinutes > 20;
        }
    }
}
