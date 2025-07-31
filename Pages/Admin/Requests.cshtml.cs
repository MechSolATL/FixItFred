using Data;
using Data.Models;

namespace Pages.Admin
{
    public class RequestsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public RequestsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<UserResponse> Responses { get; set; } = [];

        public async Task OnGetAsync()
        {
            Responses = await _context.UserResponses
                .Include(static r => r.Question)
                .Include(static r => r.ServiceRequest)
                .ToListAsync();
        }
    }
}
