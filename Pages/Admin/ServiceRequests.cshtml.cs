using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Data;
using MVP_Core.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace MVP_Core.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class ServiceRequestsModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;

        public ServiceRequestsModel(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<ServiceRequest> ServiceRequests { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; } = string.Empty;

        [BindProperty]
        public int RequestId { get; set; }

        [BindProperty]
        public string NewStatus { get; set; } = string.Empty;

        private static readonly HashSet<string> AllowedStatuses = new()
        {
            "New", "In Progress", "Scheduled", "Completed", "Cancelled", "Closed"
        };

        public async Task<IActionResult> OnGetAsync()
        {
            var query = _dbContext.ServiceRequests.AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                query = query.Where(r =>
                    (r.Phone != null && r.Phone.Contains(SearchTerm)) ||
                    (r.Email != null && r.Email.Contains(SearchTerm)) ||
                    (r.Address != null && r.Address.Contains(SearchTerm)));
            }

            ServiceRequests = await query
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return Page();
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostUpdateStatusAsync()
        {
            if (!AllowedStatuses.Contains(NewStatus))
            {
                ModelState.AddModelError(string.Empty, "Invalid status value.");
                return await OnGetAsync(); // reload page with errors
            }

            var request = await _dbContext.ServiceRequests.FindAsync(RequestId);

            if (request != null)
            {
                request.Status = NewStatus;
                await _dbContext.SaveChangesAsync();
            }

            return RedirectToPage();
        }
    }
}
