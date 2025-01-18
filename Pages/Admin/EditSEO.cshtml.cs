using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Data.Models;
using MVP_Core.Data;

namespace MVP_Core.Pages.Admin
{
    // ✅ Primary constructor used for dependency injection
    public class EditSEOModel(ApplicationDbContext dbContext) : PageModel
    {
        private readonly ApplicationDbContext _dbContext = dbContext;

        [BindProperty]
        public SEO SEO { get; set; } = new SEO();

        public void OnGet(int id)
        {
            // ✅ Using 'id' to fetch SEO data
            SEO = _dbContext.SEOs.FirstOrDefault(seo => seo.Id == id) ?? new SEO();
        }
    }
}
