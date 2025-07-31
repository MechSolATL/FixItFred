// Sprint 76.1 Patch: PageModel for Anonymous Review Trigger logs
using Data;
using Data.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;

namespace Pages.Admin.HR
{
    public class AnonymousReviewsModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public AnonymousReviewsModel(ApplicationDbContext db) { _db = db; }
        public List<AnonymousReviewFormLog> AnonymousReviewFormLogs { get; set; } = new();
        public void OnGet()
        {
            AnonymousReviewFormLogs = _db.AnonymousReviewFormLogs.OrderByDescending(a => a.TriggeredAt).Take(50).ToList();
        }
    }
}
