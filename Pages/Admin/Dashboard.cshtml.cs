using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MVP_Core.Data;
using MVP_Core.Data.Models;

namespace MVP_Core.Pages.Admin
{
    public class DashboardModel(ApplicationDbContext context) : PageModel
    {
        private readonly ApplicationDbContext _context = context;

        [BindProperty(SupportsGet = true)]
        public string? ServiceType { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Status { get; set; }

        public List<ServiceRequest> ServiceRequests { get; set; } = [];

        public async Task OnGetAsync()
        {
            var query = _context.ServiceRequests.AsQueryable();

            if (!string.IsNullOrEmpty(ServiceType))
                query = query.Where(r => r.ServiceType == ServiceType);

            if (!string.IsNullOrEmpty(Status))
                query = query.Where(r => r.Status == Status);

            ServiceRequests = await query
                .OrderByDescending(r => r.CreatedAt)

                .ToListAsync();
        }
    }
}
