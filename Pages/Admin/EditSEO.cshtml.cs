using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Data;
using MVP_Core.Data.Models;

namespace MVP_Core.Pages.Admin
{
    public class EditSEOModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;

        public EditSEOModel(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [BindProperty]
        public SEO SEO { get; set; } = new SEO();

        public IActionResult OnGet(int id)
        {
            SEO = _dbContext.SEOs.FirstOrDefault(seo => seo.Id == id);

            if (SEO == null)
            {
                TempData["Error"] = "SEO entry not found.";
                return RedirectToPage("/Admin/ManageSEO");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var existingSEO = await _dbContext.SEOs.FindAsync(SEO.Id);

            if (existingSEO == null)
            {
                TempData["Error"] = "SEO entry not found.";
                return RedirectToPage("/Admin/ManageSEO");
            }

            // Update fields
            existingSEO.Title = SEO.Title;
            existingSEO.MetaDescription = SEO.MetaDescription;
            existingSEO.Keywords = SEO.Keywords;
            existingSEO.RobotsMeta = SEO.RobotsMeta;

            await _dbContext.SaveChangesAsync();

            TempData["Message"] = "✅ SEO entry updated successfully!";
            return RedirectToPage("/Admin/ManageSEO");
        }
    }
}
