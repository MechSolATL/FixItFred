using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MVP_Core.Data;
using MVP_Core.Data.Models;

namespace MVP_Core.Pages.Admin
{
    public class ManagePagesModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;

        public ManagePagesModel(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<MVP_Core.Data.Models.Page> Pages { get; set; } = new();

        [TempData]
        public string? Message { get; set; }

        public async Task OnGetAsync()
        {
            Pages = await _dbContext.Pages
                .OrderBy(p => p.UrlPath)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostToggleVisibilityAsync(int id)
        {
            var page = await _dbContext.Pages.FindAsync(id);

            if (page == null)
            {
                return NotFound();
            }

            page.IsPublic = !page.IsPublic;
            page.UpdatedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            Message = "✅ Page visibility updated!";

            return RedirectToPage();
        }
    }
}
