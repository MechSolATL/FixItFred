using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Data;
using MVP_Core.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace MVP_Core.Pages.Admin
{
    public class ManageSEOModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;

        public List<SEO> SEOs { get; set; } = new();

        public ManageSEOModel(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task OnGetAsync()
        {
            SEOs = await _dbContext.SEOs
                .OrderBy(s => s.PageName)
                .ToListAsync();
        }
    }
}
