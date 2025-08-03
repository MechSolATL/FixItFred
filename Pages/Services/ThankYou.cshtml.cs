using Data;
using Data.Models;
using Data.Models.Seo;
using Data.Models.ViewModels;
using Helpers;
using Services;
using Services.Email;

namespace Pages.Services
{
    [IgnoreAntiforgeryToken]
    public class ThankYouModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly QuestionService _questionService;
        private readonly EmailService _emailService;
        private readonly ISeoService _seoService;
        private readonly IDeviceResolver _deviceResolver;

        public List<SubmittedAnswerViewModel> SubmittedAnswers { get; set; } = [];

        public ThankYouModel(
            ApplicationDbContext dbContext,
            QuestionService questionService,
            EmailService emailService,
            ISeoService seoService,
            IDeviceResolver deviceResolver)
        {
            _dbContext = dbContext;
            _questionService = questionService;
            _emailService = emailService;
            _seoService = seoService;
            _deviceResolver = deviceResolver;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            SeoMeta? SeoMeta = await _seoService.GetSeoMetaAsync("ThankYou");
            ViewData["Title"] = SeoMeta?.Title ?? "Thank You!";
            ViewData["MetaDescription"] = SeoMeta?.MetaDescription ?? "Your service request was successfully submitted.";
            ViewData["Keywords"] = SeoMeta?.Keywords ?? "service request confirmation, thank you";
            ViewData["Robots"] = SeoMeta?.Robots ?? "noindex, nofollow";
            ViewData["DeviceType"] = _deviceResolver.GetDeviceType(HttpContext);

            string? sessionId = HttpContext.Session.GetString("CurrentSessionID");
            if (string.IsNullOrEmpty(sessionId))
            {
                TempData["ErrorMessage"] = "Session expired or invalid.";
                return RedirectToPage("/Error");
            }

            List<UserResponse> userResponses = await _dbContext.UserResponses
                .Where(r => r.SessionID == sessionId)
                .OrderBy(r => r.CreatedAt)
                .ToListAsync();

            if (!userResponses.Any())
            {
                TempData["ErrorMessage"] = "No saved responses found.";
                return RedirectToPage("/Error");
            }

            foreach (UserResponse response in userResponses)
            {
                Question? question = await _dbContext.Questions.FirstOrDefaultAsync(q => q.Id == response.QuestionId);
                if (question != null)
                {
                    SubmittedAnswers.Add(new SubmittedAnswerViewModel
                    {
                        QuestionText = question.Text,
                        Response = response.Response
                    });
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostSubmitAsync()
        {
            ServiceRequestSession? session = HttpContext.Session.GetObject<ServiceRequestSession>("ServiceRequest");

            if (session == null || string.IsNullOrEmpty(session.CustomerName) || string.IsNullOrEmpty(session.PhoneNumber))
            {
                TempData["ErrorMessage"] = "Session expired or incomplete.";
                return RedirectToPage("/Error");
            }

            using Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                string sessionId = HttpContext.Session.Id;

                Data.Models.ServiceRequest newRequest = new()
                {
                    CustomerName = session.CustomerName,
                    Phone = session.PhoneNumber,
                    Email = session.Email,
                    Address = session.Address,
                    ServiceType = session.ServiceType,
                    Details = string.Join("; ", session.Answers.Select(a => $"QID {a.Key}: {a.Value.Answer}")),
                    CreatedAt = DateTime.UtcNow,
                    Status = "New",
                    SessionId = sessionId
                };

                _ = _dbContext.ServiceRequests.Add(newRequest);
                _ = _dbContext.SaveChanges();

                List<UserResponse> userResponses = session.Answers.Select(a => new UserResponse
                {
                    SessionID = sessionId,
                    QuestionId = a.Key,
                    Response = a.Value.Answer,
                    CreatedAt = a.Value.AnsweredAt
                }).ToList();

                _dbContext.UserResponses.AddRange(userResponses);
                _ = _dbContext.SaveChanges();

                await transaction.CommitAsync();

                try
                {
                    await _emailService.SendServiceRequestConfirmationToCustomerAsync(newRequest);
                    await _emailService.NotifyAdminOfNewRequestAsync(newRequest);
                }
                catch
                {
                    // Non-blocking email failure
                }

                HttpContext.Session.Clear();
                return RedirectToPage("/Shared/ThankYouSuccess");
            }
            catch
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = "An error occurred while submitting your request.";
                return RedirectToPage("/Error");
            }
        }
    }
}
