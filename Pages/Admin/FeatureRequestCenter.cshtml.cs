using MVP_Core.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Data;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System;
using System.IO;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

namespace MVP_Core.Pages.Admin
{
    // Sprint 83.4: Feedback center
    [Authorize(Roles = "Admin,User")]
    public class FeatureRequestCenterModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public FeatureRequestCenterModel(ApplicationDbContext db, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
        }

        [BindProperty]
        public string FeedbackCategory { get; set; } = "General";
        [BindProperty]
        public string SuggestionText { get; set; } = string.Empty;
        [BindProperty]
        public string UrgencyLevel { get; set; } = "Low";
        [BindProperty]
        public IFormFile? Attachment { get; set; }
        [BindProperty]
        public string UserName { get; set; } = string.Empty;
        [BindProperty]
        public string Role { get; set; } = string.Empty;
        public bool SubmitSuccess { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;

        public void OnGet()
        {
            UserName = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Unknown";
            Role = _httpContextAccessor.HttpContext?.User?.IsInRole("Admin") == true ? "Admin" : "User";
        }

        public async Task<IActionResult> OnPostAsync()
        {
            UserName = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Unknown";
            Role = _httpContextAccessor.HttpContext?.User?.IsInRole("Admin") == true ? "Admin" : "User";
            string? attachmentPath = null;
            if (Attachment != null && Attachment.Length > 0)
            {
                var uploads = Path.Combine("wwwroot", "FeatureSuggestionUploads");
                Directory.CreateDirectory(uploads);
                var fileName = $"{Guid.NewGuid()}_{Attachment.FileName}";
                var filePath = Path.Combine(uploads, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await Attachment.CopyToAsync(stream);
                }
                attachmentPath = $"/FeatureSuggestionUploads/{fileName}";
            }
            var suggestion = new FeatureSuggestion
            {
                UserName = UserName,
                Role = Role,
                SuggestionText = SuggestionText,
                UrgencyLevel = UrgencyLevel,
                FeedbackCategory = FeedbackCategory,
                SubmittedAt = DateTime.UtcNow,
                AttachmentPath = attachmentPath
            };
            try
            {
                _db.FeatureSuggestions.Add(suggestion);
                await _db.SaveChangesAsync();
                SubmitSuccess = true;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error submitting feedback: {ex.Message}";
            }
            return Page();
        }
    }
}
