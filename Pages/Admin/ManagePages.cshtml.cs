namespace MVP_Core.Pages.Admin
{
    public class ManagePagesModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;

        public ManagePagesModel(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<MVP_Core.Data.Models.Page> Pages { get; set; } = [];

        [TempData]
        public string? Message { get; set; }

        public async Task OnGetAsync()
        {
            Pages = await _dbContext.Pages
                .OrderBy(static p => p.UrlPath)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostToggleVisibilityAsync(int id)
        {
            Data.Models.Page? page = await _dbContext.Pages.FindAsync(id);

            if (page == null)
            {
                return NotFound();
            }

            page.IsPublic = !page.IsPublic;
            page.UpdatedAt = DateTime.UtcNow;

            _ = _dbContext.SaveChanges();
            Message = "? Page visibility updated!";

            return RedirectToPage();
        }
    }
}
