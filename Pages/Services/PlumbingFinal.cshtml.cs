// ? File: E:\source\MVP-Core\Pages\Services\PlumbingFinal.cshtml.cs

using Data;
using Data.Models;
using Helpers;

namespace Pages.Services
{
    [ValidateAntiForgeryToken] // ? FIXED: applied here, not on method
    public class PlumbingFinalModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;

        public PlumbingFinalModel(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [BindProperty]
        public string Message { get; set; } = string.Empty;

        [BindProperty]
        public bool IsSuccess { get; set; } = false;

        public ServiceRequestSession? SessionData { get; set; }

        private const string SessionKey = "ServiceRequest";
        private const string SessionStartKey = "ServiceRequestStart";

        public IActionResult OnGet()
        {
            SessionData = HttpContext.Session.GetObject<ServiceRequestSession>(SessionKey);

            if (SessionData == null || !SessionData.PhoneVerified)
            {
                TempData["ErrorMessage"] = "Session expired or verification incomplete.";
                return RedirectToPage("/Services/Plumbing");
            }

            if (IsSessionExpired())
            {
                TempData["ErrorMessage"] = "Session timed out.";
                HttpContext.Session.Clear();
                return RedirectToPage("/Services/Plumbing");
            }

            return Page();
        }

        public IActionResult OnPostSubmit()
        {
            SessionData = HttpContext.Session.GetObject<ServiceRequestSession>(SessionKey);

            if (SessionData == null || !SessionData.PhoneVerified)
            {
                TempData["ErrorMessage"] = "Session expired or invalid.";
                return RedirectToPage("/Services/Plumbing");
            }

            string details = string.Join("\n", SessionData.Answers.Select(static a =>
                $"QID {a.Key}: {a.Value.Answer?.Trim()} (Answered at {a.Value.AnsweredAt:yyyy-MM-dd HH:mm:ss})"));

            ServiceRequest serviceRequest = new()
            {
                CustomerName = SessionData.CustomerName,
                Phone = SessionData.PhoneNumber,
                Email = SessionData.Email,
                Address = SessionData.Address,
                ServiceType = SessionData.ServiceType,
                Details = details,
                CreatedAt = DateTime.UtcNow,
                Status = "Pending",
                SessionId = HttpContext.Session.Id
            };

            _ = _dbContext.ServiceRequests.Add(serviceRequest);
            _ = _dbContext.SaveChanges();

            HttpContext.Session.Clear();
            TempData["SuccessMessage"] = "Service request submitted successfully!";
            return RedirectToPage("/Services/ThankYou");
        }

        private bool IsSessionExpired()
        {
            string? sessionStartString = HttpContext.Session.GetString(SessionStartKey);
            return !DateTime.TryParse(sessionStartString, out DateTime sessionStart)
                || (DateTime.UtcNow - sessionStart).TotalMinutes > 20;
        }
    }
}
