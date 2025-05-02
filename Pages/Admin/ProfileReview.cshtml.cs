using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Data;
using MVP_Core.Data.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace MVP_Core.Pages.Admin
{
    public class ProfileReviewModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;

        public ProfileReviewModel(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [BindProperty]
        [Required]
        [StringLength(100)]
        public string Username { get; set; } = string.Empty;

        [BindProperty]
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;

        [BindProperty]
        [StringLength(500)]
        public string ReviewNotes { get; set; } = string.Empty;

        [BindProperty]
        [StringLength(10)]
        public string VerificationCode { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync()
        {
            var username = User.Identity?.Name;
            var adminUser = await _dbContext.AdminUsers.FirstOrDefaultAsync(u => u.Username == username);

            if (adminUser == null)
            {
                return RedirectToPage("/AccessDenied");
            }

            Username = adminUser.Username;
            Email = adminUser.Email;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var username = User.Identity?.Name;
            var adminUser = await _dbContext.AdminUsers.FirstOrDefaultAsync(u => u.Username == username);

            if (adminUser == null)
            {
                return RedirectToPage("/AccessDenied");
            }

            adminUser.LastProfileReviewDate = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();

            return RedirectToPage("/Admin/Index");
        }
    }
}
