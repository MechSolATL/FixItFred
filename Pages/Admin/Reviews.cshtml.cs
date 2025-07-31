using Data;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;

namespace Pages.Admin
{
    // Sprint 84.6 — Review Display Integration
    public class ReviewsModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public ReviewsModel(ApplicationDbContext db) { _db = db; }

        public IQueryable<CustomerReview> Reviews => _db.CustomerReviews.OrderByDescending(r => r.SubmittedAt);

        public IActionResult OnPostTogglePublic(int id)
        {
            var review = _db.CustomerReviews.FirstOrDefault(r => r.Id == id);
            if (review != null)
            {
                review.IsPublic = !review.IsPublic;
                _db.SaveChanges();
            }
            return RedirectToPage();
        }

        public IActionResult OnPostApprove(int id)
        {
            var review = _db.CustomerReviews.FirstOrDefault(r => r.Id == id);
            if (review != null)
            {
                // Example: Approve logic (could set a status or flag)
                review.IsFlagged = false;
                _db.SaveChanges();
            }
            return RedirectToPage();
        }

        public IActionResult OnPostReject(int id)
        {
            var review = _db.CustomerReviews.FirstOrDefault(r => r.Id == id);
            if (review != null)
            {
                // Example: Reject logic (could set a status or flag)
                review.IsFlagged = true;
                _db.SaveChanges();
            }
            return RedirectToPage();
        }
    }
}
