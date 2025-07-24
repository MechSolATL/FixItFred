namespace MVP_Core.Pages.Admin
{
    public class ServiceRequestsModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public ServiceRequestsModel(ApplicationDbContext db)
        {
            _db = db;
        }

        [BindProperty(SupportsGet = true)]
        public string? StatusFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? ServiceTypeFilter { get; set; }

        public List<ServiceRequest> Requests { get; set; } = [];

        public async Task<IActionResult> OnGetAsync()
        {
            IQueryable<ServiceRequest> query = _db.ServiceRequests.AsQueryable();

            if (!string.IsNullOrEmpty(StatusFilter))
            {
                query = query.Where(r => r.Status == StatusFilter);
            }

            if (!string.IsNullOrEmpty(ServiceTypeFilter))
            {
                query = query.Where(r => r.ServiceType == ServiceTypeFilter);
            }

            Requests = await query
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return Page();
        }
    }
}
