using MVP_Core.Data;
using MVP_Core.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace MVP_Core.Services
{
    public class ServiceRequestService
    {
        private readonly ApplicationDbContext _context;

        public ServiceRequestService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Creates a new service request and returns its unique ID.
        /// </summary>
        public async Task<int> CreateServiceRequestAsync(
            string customerName,
            string email,
            string? phone,
            string? address,
            string serviceType,
            string? serviceSubtype,
            string details,
            string? sessionId,
            bool isUrgent = false
        )
        {
            if (string.IsNullOrWhiteSpace(customerName))
                throw new ArgumentException("Customer name is required.", nameof(customerName));

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required.", nameof(email));

            if (string.IsNullOrWhiteSpace(serviceType))
                throw new ArgumentException("Service type is required.", nameof(serviceType));

            var request = new ServiceRequest
            {
                CustomerName = customerName.Trim(),
                Email = email.Trim(),
                Phone = phone?.Trim(),
                Address = address?.Trim(),
                ServiceType = serviceType.Trim(),
                ServiceSubtype = serviceSubtype?.Trim(),
                Details = details.Trim(),
                SessionId = sessionId?.Trim(),
                IsUrgent = isUrgent,
                CreatedAt = DateTime.UtcNow,
                Status = "Pending"
            };

            _context.ServiceRequests.Add(request);
            await _context.SaveChangesAsync();

            return request.Id;
        }

        /// <summary>
        /// Optional: Retrieve request by ID (useful for confirmation page)
        /// </summary>
        public async Task<ServiceRequest?> GetRequestByIdAsync(int requestId)
        {
            return await _context.ServiceRequests
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == requestId);
        }
    }
}
