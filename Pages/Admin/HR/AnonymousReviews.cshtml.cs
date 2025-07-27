// Sprint 76.1 Patch: PageModel for Anonymous Review Trigger logs
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Data;
using MVP_Core.Data.Models;
using System.Collections.Generic;
using System.Linq;

namespace MVP_Core.Pages.Admin.HR
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
