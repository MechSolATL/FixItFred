using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Data;
using MVP_Core.Data.Models;
using MVP_Core.Helpers;
using System.Linq;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Http;
using MVP_Core.Models;

namespace MVP_Core.Pages.Services
{
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

        public async Task<IActionResult> OnGetAsync()
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

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostSubmitAsync()
        {
            SessionData = HttpContext.Session.GetObject<ServiceRequestSession>(SessionKey);

            if (SessionData == null || !SessionData.PhoneVerified)
            {
                TempData["ErrorMessage"] = "Session expired or invalid.";
                return RedirectToPage("/Services/Plumbing");
            }

            var details = string.Join("\n", SessionData.Answers.Select(a =>
                $"QID {a.Key}: {a.Value.Answer?.Trim()} (Answered at {a.Value.AnsweredAt:yyyy-MM-dd HH:mm:ss})"));

            var serviceRequest = new ServiceRequest
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

            _dbContext.ServiceRequests.Add(serviceRequest);
            await _dbContext.SaveChangesAsync();

            HttpContext.Session.Clear();
            TempData["SuccessMessage"] = "Service request submitted successfully!";

            return RedirectToPage("/Services/ThankYou");
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
