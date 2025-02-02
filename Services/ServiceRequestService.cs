using MVP_Core.Data;
using MVP_Core.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace MVP_Core.Services
{
    public class ServiceRequestService(ApplicationDbContext context)
    {
        private readonly ApplicationDbContext _context = context;

        public async Task SaveRequestAsync(ServiceRequest request)
        {
            _context.ServiceRequests.Add(request);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ServiceRequest>> GetPendingRequestsAsync()
        {
            return await _context.ServiceRequests
                .Where(r => r.Status == "Pending")
                .ToListAsync();
        }
    }
}
