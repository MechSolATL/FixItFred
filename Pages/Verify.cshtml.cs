// =========================
// File: Pages/Verify.cshtml.cs
// =========================
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Data;
using MVP_Core.Data.Models;
using MVP_Core.Services;
using MVP_Core.Services.Email;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;

namespace MVP_Core.Pages
{
    public class VerifyModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly ISeoService _seoService;
        private readonly EmailService _emailService;
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _httpClientFactory;

        public VerifyModel(ApplicationDbContext db, ISeoService seoService, EmailService emailService, IConfiguration config, IHttpClientFactory httpClientFactory)
        {
            _db = db;
            _seoService = seoService;
            _emailService = emailService;
            _config = config;
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty]
        public VerificationInputModel Input { get; set; } = new();
        [BindProperty]
        public string HCaptchaToken { get; set; } = string.Empty;
        public string? Message { get; set; }
        public bool IsSuccess { get; set; } = false;
        public ServiceRequest? VerifiedRequest { get; set; }
        public List<string> FirstAnswers { get; set; } = new();
        public int FailedAttempts { get; set; } = 0;
        public int ResendCount { get; set; } = 0;
        public bool ResendCooldown { get; set; } = false;

        public async Task<IActionResult> OnGetAsync()
        {
            var seo = await _seoService.GetSeoByPageNameAsync("Verify");
            if (seo != null)
            {
                ViewData["Title"] = seo.Title;
                ViewData["MetaDescription"] = seo.MetaDescription;
                ViewData["Keywords"] = seo.Keywords;
                ViewData["Robots"] = seo.Robots ?? "noindex, nofollow";
            }
            ResendCount = HttpContext?.Session?.GetInt32("ResendCount") ?? 0;
            ResendCooldown = (HttpContext?.Session?.GetInt32("ResendCooldown") ?? 0) == 1;
            return Page();
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
                Message = "hCaptcha validation failed. Please try again.";
                return Page();
            }
            if (!ModelState.IsValid)
            {
                Message = "Invalid input.";
                return Page();
            }

            // Track failed attempts in session
            FailedAttempts = HttpContext?.Session?.GetInt32("VerifyFailed") ?? 0;

            var record = await _db.EmailVerifications
                .FirstOrDefaultAsync(e => e.Email == Input.Email && !e.IsVerified);

            if (record == null)
            {
                Message = "No verification record found.";
                FailedAttempts++;
                HttpContext?.Session?.SetInt32("VerifyFailed", FailedAttempts);
                return Page();
            }

            if ((record.Code?.Trim() ?? string.Empty) != (Input.Code?.Trim() ?? string.Empty))
            {
                Message = "Incorrect verification code.";
                FailedAttempts++;
                HttpContext?.Session?.SetInt32("VerifyFailed", FailedAttempts);
                return Page();
            }

            record.IsVerified = true;
            await _db.SaveChangesAsync();

            HttpContext?.Session?.SetString("IsEmailVerified", "true");
            HttpContext?.Session?.SetString("VerifiedEmail", Input.Email ?? string.Empty);
            IsSuccess = true;
            Message = "? Email verified successfully.";

            // Find most recent ServiceRequest for this email
            VerifiedRequest = await _db.ServiceRequests
                .Where(r => r.Email == Input.Email)
                .OrderByDescending(r => r.CreatedAt)
                .FirstOrDefaultAsync();

            // Parse first 2 answers from Details (if present)
            FirstAnswers = new List<string>();
            if (VerifiedRequest != null && !string.IsNullOrWhiteSpace(VerifiedRequest.Details))
            {
                var lines = VerifiedRequest.Details.Split('\n');
                foreach (var line in lines.Take(2))
                {
                    if (!string.IsNullOrWhiteSpace(line))
                        FirstAnswers.Add(line.Trim());
                }
            }
            return Page();
        }

        public async Task<IActionResult> OnPostResendAsync()
        {
            if (!await ValidateHCaptchaAsync(HCaptchaToken))
            {
                ModelState.AddModelError(string.Empty, "hCaptcha validation failed. Please try again.");
                Message = "hCaptcha validation failed. Please try again.";
                return Page();
            }
            // Throttle: max 3 resends per session, 60s cooldown
            ResendCount = HttpContext?.Session?.GetInt32("ResendCount") ?? 0;
            if (ResendCount >= 3)
            {
                Message = "You have reached the maximum number of resend attempts. Please try again later or contact support.";
                return Page();
            }
            if ((HttpContext?.Session?.GetInt32("ResendCooldown") ?? 0) == 1)
            {
                Message = "Please wait before requesting another code.";
                return Page();
            }
            if (string.IsNullOrWhiteSpace(Input.Email))
            {
                Message = "Please enter your email address to resend the code.";
                return Page();
            }

            // Remove old code and generate new one
            var existing = await _db.EmailVerifications.FirstOrDefaultAsync(e => e.Email == Input.Email);
            if (existing != null)
            {
                _db.EmailVerifications.Remove(existing);
                await _db.SaveChangesAsync();
            }
            // Generate new code
            var code = new Random().Next(100000, 999999).ToString();
            var verification = new EmailVerification
            {
                Email = Input.Email?.Trim() ?? string.Empty,
                Code = code,
                Expiration = DateTime.UtcNow.AddMinutes(15),
                IsVerified = false
            };
            _db.EmailVerifications.Add(verification);
            await _db.SaveChangesAsync();

            // Send email
            string verifyUrl = Url.Page("/Verify", null, new { email = Input.Email }, Request.Scheme)!;
            await _emailService.SendVerificationEmailAsync(Input.Email ?? string.Empty, verifyUrl);

            // Set cooldown and increment resend count
            HttpContext?.Session?.SetInt32("ResendCount", ResendCount + 1);
            HttpContext?.Session?.SetInt32("ResendCooldown", 1);
            _ = Task.Run(async () => {
                await Task.Delay(60000);
                HttpContext?.Session?.SetInt32("ResendCooldown", 0);
            });

            Message = "A new verification code has been sent to your email.";
            return Page();
        }
    }

    public class VerificationInputModel
    {
        [Required, EmailAddress, MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required, MaxLength(10)]
        public string Code { get; set; } = string.Empty;
    }
}
