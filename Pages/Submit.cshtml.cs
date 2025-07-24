using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Data;
using MVP_Core.Data.Models;
using MVP_Core.Helpers;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using MVP_Core.Hubs;
using MVP_Core.Services;

namespace MVP_Core.Pages
{
    [ValidateAntiForgeryToken]
    public class SubmitModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly SessionTracker _session;
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHubContext<RequestHub> _hub;
        private readonly ISeoService _seoService;
        private readonly IDeviceResolver _deviceResolver;

        public SubmitModel(ApplicationDbContext dbContext, SessionTracker session, IConfiguration config, IHttpClientFactory httpClientFactory, IHubContext<RequestHub> hub, ISeoService seoService, IDeviceResolver deviceResolver)
        {
            _dbContext = dbContext;
            _session = session;
            _config = config;
            _httpClientFactory = httpClientFactory;
            _hub = hub;
            _seoService = seoService;
            _deviceResolver = deviceResolver;
        }

        public ServiceRequestSession? SessionData { get; set; }
        [BindProperty]
        public string HCaptchaToken { get; set; } = string.Empty;

        public async Task OnGetAsync()
        {
            var seo = await _seoService.GetSeoByPageNameAsync("Submit");
            ViewData["Title"] = seo?.Title ?? "Submit Service Request";
            ViewData["MetaDescription"] = seo?.MetaDescription;
            ViewData["Keywords"] = seo?.Keywords;
            ViewData["Robots"] = seo?.Robots;
            ViewData["DeviceType"] = _deviceResolver.GetDeviceType(HttpContext);

            // Use SessionTracker for session validation
            if (!_session.AnswersSubmitted && _session.ServiceRequestId == null)
            {
                TempData["Error"] = "Session expired or no request data found.";
                Response.Redirect("/Services/Plumbing");
                return;
            }
            // Optionally, you can still load the full session object for display
            SessionData = HttpContext.Session.GetObject<ServiceRequestSession>("ServiceRequest");
        }

        private async Task<bool> ValidateHCaptchaAsync(string token)
        {
            var secret = _config["hCaptcha:SecretKey"];
            if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(secret))
                return false;
            var client = _httpClientFactory.CreateClient();
            var values = new Dictionary<string, string>
            {
                { "secret", secret },
                { "response", token }
            };
            var content = new FormUrlEncodedContent(values);
            var response = await client.PostAsync("https://hcaptcha.com/siteverify", content);
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonDocument.Parse(json);
            return result.RootElement.TryGetProperty("success", out var success) && success.GetBoolean();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!await ValidateHCaptchaAsync(HCaptchaToken))
            {
                ModelState.AddModelError(string.Empty, "hCaptcha validation failed. Please try again.");
                await OnGetAsync();
                return Page();
            }
            // Use SessionTracker for session validation
            if (!_session.AnswersSubmitted && _session.ServiceRequestId == null)
            {
                TempData["Error"] = "Session expired or no request data found.";
                return RedirectToPage("/Services/Plumbing");
            }
            SessionData = HttpContext.Session.GetObject<ServiceRequestSession>("ServiceRequest");
            if (SessionData == null)
            {
                TempData["Error"] = "Session expired or no request data found.";
                return RedirectToPage("/Services/Plumbing");
            }

            // Save to DB
            var details = string.Join("\n", SessionData.Answers.Select(a => $"QID {a.Key}: {a.Value.Answer} (Answered at {a.Value.AnsweredAt:yyyy-MM-dd HH:mm:ss})"));
            var request = new ServiceRequest
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
            _dbContext.ServiceRequests.Add(request);
            _dbContext.SaveChanges();

            // Broadcast to SignalR hub for dispatcher dashboard
            await _hub.Clients.All.SendAsync("NewRequest");

            // Update SessionTracker flags
            _session.AnswersSubmitted = true;
            _session.ServiceRequestId = request.Id;
            _session.CustomerName = request.CustomerName;
            _session.CustomerEmail = request.Email;
            _session.IsVerified = true;

            HttpContext.Session.Clear();
            return RedirectToPage("/Services/ThankYou");
        }
    }
}
