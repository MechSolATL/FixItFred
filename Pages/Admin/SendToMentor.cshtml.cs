using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Services.Mentorship;
using Services.Analytics;
using Data.Models;
using Models;
using Data;

namespace Pages.Admin
{
    // Sprint 86.6 — Send To Mentor PageModel
    public class SendToMentorModel : PageModel
    {
        private readonly MentorSelectorEngine _mentorSelector;
        private readonly ActionLogService _actionLogService;
        private readonly ApplicationDbContext _db;
        public SendToMentorModel(MentorSelectorEngine mentorSelector, ActionLogService actionLogService, ApplicationDbContext db)
        {
            _mentorSelector = mentorSelector;
            _actionLogService = actionLogService;
            _db = db;
        }
        public List<AdminUser> SuggestedMentors { get; set; } = new();
        public List<UserActionLog> RecentFlow { get; set; } = new();
        [BindProperty]
        public int SelectedMentorId { get; set; }
        [BindProperty]
        public string HelpMessage { get; set; } = string.Empty;
        public AdminUser? Mentor { get; set; }
        public AdminUser? RequestingUser { get; set; }
        public async Task OnGetAsync()
        {
            int userId = int.Parse(User.Identity?.Name ?? "0");
            RequestingUser = await _db.AdminUsers.FindAsync(userId);
            SuggestedMentors = await _mentorSelector.GetSuggestedMentorsForUser(userId);
            RecentFlow = await _actionLogService.CaptureRecentFlowAsync(userId);
        }
        public async Task<IActionResult> OnPostAsync()
        {
            int userId = int.Parse(User.Identity?.Name ?? "0");
            var mentor = await _db.AdminUsers.FindAsync(SelectedMentorId);
            var flow = await _actionLogService.CaptureRecentFlowAsync(userId);
            // Save mentor request (could be a new table, or log as MentorReplyLog with empty response)
            var req = new MentorReplyLog
            {
                MentorId = SelectedMentorId,
                RequestingUserId = userId,
                Timestamp = DateTime.UtcNow,
                ResponseText = "[REQUEST] " + HelpMessage,
                FlowId = 0 // Could be enhanced to link to a flow session
            };
            _db.MentorReplyLogs.Add(req);
            await _db.SaveChangesAsync();
            return RedirectToPage("/Admin/MentorInbox");
        }
    }
}
