using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Data;
using MVP_Core.Data.Models;
using MVP_Core.Models;
using MVP_Core.Data.Models.ViewModels;
using MVP_Core.Services.Email;
using Microsoft.EntityFrameworkCore;

namespace MVP_Core.Pages.Services
{
    [IgnoreAntiforgeryToken] // 🛡️ Fix: Apply here globally to the PageModel, NOT individual handlers
    public class ThankYouModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly QuestionService _questionService;
        private readonly EmailService _emailService;

        public List<SubmittedAnswerViewModel> SubmittedAnswers { get; set; } = [];

        public ThankYouModel(ApplicationDbContext dbContext, QuestionService questionService, EmailService emailService)
        {
            _dbContext = dbContext;
            _questionService = questionService;
            _emailService = emailService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var sessionId = HttpContext.Session.GetString("CurrentSessionID");

            if (string.IsNullOrEmpty(sessionId))
            {
                TempData["ErrorMessage"] = "Session expired or invalid.";
                return RedirectToPage("/Error");
            }

            var userResponses = await _dbContext.UserResponses
                .Where(r => r.SessionID == sessionId)
                .OrderBy(r => r.CreatedAt)
                .ToListAsync();

            if (!userResponses.Any())
            {
                TempData["ErrorMessage"] = "No saved responses found.";
                return RedirectToPage("/Error");
            }

            foreach (var response in userResponses)
            {
                var question = await _dbContext.Questions.FirstOrDefaultAsync(q => q.Id == response.QuestionId);
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
            var session = HttpContext.Session.GetObject<ServiceRequestSession>("ServiceRequest");

            if (session == null || string.IsNullOrEmpty(session.CustomerName) || string.IsNullOrEmpty(session.PhoneNumber))
            {
                TempData["ErrorMessage"] = "Session expired or incomplete.";
                return RedirectToPage("/Error");
            }

            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var sessionId = HttpContext.Session.Id;

                var newRequest = new ServiceRequest
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

                _dbContext.ServiceRequests.Add(newRequest);
                await _dbContext.SaveChangesAsync();

                var userResponses = session.Answers.Select(answer => new UserResponse
                {
                    SessionID = sessionId,
                    QuestionId = answer.Key,
                    Response = answer.Value.Answer,
                    CreatedAt = answer.Value.AnsweredAt
                }).ToList();

                _dbContext.UserResponses.AddRange(userResponses);
                await _dbContext.SaveChangesAsync();

                await transaction.CommitAsync();

                try
                {
                    await _emailService.SendServiceRequestConfirmationToCustomerAsync(newRequest);
                    await _emailService.NotifyAdminOfNewRequestAsync(newRequest);
                }
                catch
                {
                    // 🛑 Email failure should not block user flow.
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
