using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Data;
using MVP_Core.Data.Models;

namespace MVP_Core.Pages.Admin
{
    public class QuickTestModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public QuickTestModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var testRequest = new ServiceRequest
            {
                CustomerName = "Test User",
                Email = "test@example.com",
                Phone = "123-456-7890",
                Address = "123 Fake St, Nowhere, USA",
                ServiceType = "Plumbing",
                ServiceSubtype = "Leak Repair",
                Details = "There is a small leak under the sink.",
                CreatedAt = DateTime.UtcNow,
                Status = "Pending",
                SessionId = Guid.NewGuid().ToString()
            };

            _context.ServiceRequests.Add(testRequest);
            await _context.SaveChangesAsync();

            return Content("✅ Test Service Request Saved Successfully!");
        }
    }
}
