using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Data.Models;
using Models;
using Data;

namespace Pages.Admin
{
    // Sprint 86.6 — Mentor Inbox PageModel
    public class MentorInboxModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public MentorInboxModel(ApplicationDbContext db)
        {
            _db = db;
        }
        public List<MentorReplyLog> Requests { get; set; } = new();
        public List<AdminUser> Users { get; set; } = new();
        [BindProperty]
        public int ReplyId { get; set; }
        [BindProperty]
        public string ResponseText { get; set; } = string.Empty;
        [BindProperty]
        public bool Resolved { get; set; }
        public async Task OnGetAsync()
        {
            int mentorId = int.Parse(User.Identity?.Name ?? "0");
            Requests = await _db.MentorReplyLogs
                .Where(r => r.MentorId == mentorId && r.ResponseText.StartsWith("[REQUEST]"))
                .OrderByDescending(r => r.Timestamp)
                .ToListAsync();
            Users = await _db.AdminUsers.ToListAsync();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            var reply = await _db.MentorReplyLogs.FindAsync(ReplyId);
            if (reply != null)
            {
                reply.ResponseText = ResponseText;
                if (Resolved)
                    reply.ResponseText += " [RESOLVED]";
                reply.Timestamp = DateTime.UtcNow;
                await _db.SaveChangesAsync();
            }
            return RedirectToPage();
        }
    }
}
