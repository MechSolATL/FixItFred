namespace MVP_Core.Pages.Admin
{
    public class ClicksModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;

        public List<PageVisit> ClickLogs { get; set; } = [];

        public ClicksModel(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task OnGetAsync()
        {
            ClickLogs = await _dbContext.PageVisits
                .OrderByDescending(static x => x.VisitTimestamp)
                .Take(500)
                .ToListAsync();
        }
    }
}
