// Sprint 26.5 Patch Log: CS860x/CS8625/CS1998/CS0219 fixes — Nullability, async, and unused variable corrections for Nova review.
// FixItFred: Sprint 30D.2 — CS860x nullability audit 2024-07-25
// Added null checks and safe navigation for all nullable references per CS8601, CS8602, CS8603, CS8604
// Each change is marked with FixItFred comment and timestamp
// Sprint 78.2: Hardened for CS860X
// Sprint 81: Null safety hardening for ServiceRequestService.cs

using MVP_Core.Services.Admin;
using MVP_Core.Services.Dispatch;

namespace MVP_Core.Services
{
    public class ServiceRequestService
    {
        private readonly ApplicationDbContext _context;
        private readonly DispatcherService _dispatcherService;
        private readonly NotificationDispatchEngine _dispatchEngine;

        public ServiceRequestService(ApplicationDbContext context, DispatcherService dispatcherService, NotificationDispatchEngine dispatchEngine)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dispatcherService = dispatcherService ?? throw new ArgumentNullException(nameof(dispatcherService));
            _dispatchEngine = dispatchEngine ?? throw new ArgumentNullException(nameof(dispatchEngine));
        }

        /// <summary>
        /// Creates a new service request and returns its unique ID.
        /// </summary>
        public int CreateServiceRequest(
            string customerName,
            string email,
            string? phone,
            string? address,
            string serviceType,
            string? serviceSubtype,
            string details,
            string? sessionId,
            bool isUrgent = false,
            string? zone = null,
            int delayMinutes = 0
        )
        {
            if (string.IsNullOrWhiteSpace(customerName))
            {
                throw new ArgumentException("Customer name is required.", nameof(customerName));
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("Email is required.", nameof(email));
            }

            if (string.IsNullOrWhiteSpace(serviceType))
            {
                throw new ArgumentException("Service type is required.", nameof(serviceType));
            }

            ServiceRequest request = new()
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
            _context.SaveChanges();

            var safeZone = zone ?? string.Empty;
            var tech = _dispatcherService.FindAvailableTechnicianForZone(safeZone);
            if (tech == null)
            {
                return request.Id;
            }
            // Convert Data.Models.TechnicianProfileDto to TechnicianStatusDto for PredictETA
            var techStatus = new MVP_Core.Models.Admin.TechnicianStatusDto {
                TechnicianId = tech.Id,
                Name = tech.FullName ?? string.Empty,
                Status = tech.Specialty ?? string.Empty,
                DispatchScore = 100, // Default/mock value
                LastPing = DateTime.UtcNow,
                AssignedJobs = 0,
                LastUpdate = DateTime.UtcNow
            };
            var eta = _dispatcherService.PredictETA(techStatus, safeZone, delayMinutes)?.GetAwaiter().GetResult();
            var entry = new ScheduleQueue
            {
                TechnicianId = tech.Id,
                AssignedTechnicianName = tech.FullName ?? string.Empty, // FixItFred: Sprint 30D.2 — Safe fallback for FullName 2024-07-25
                TechnicianStatus = "Pending",
                ServiceRequestId = request.Id,
                Zone = safeZone,
                ScheduledFor = DateTime.UtcNow,
                EstimatedArrival = eta,
                Status = ScheduleStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };
            _context.ScheduleQueues.Add(entry);
            _context.SaveChanges();
            _dispatchEngine.BroadcastETAAsync(safeZone, $"Technician {tech.FullName ?? "Unknown"} ETA: {eta:t}");

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

        /// <summary>
        /// Optional: Retrieve completed requests by customer email (for dispute center)
        /// </summary>
        public List<ServiceRequest> GetCompletedRequestsByCustomer(string customerEmail)
        {
            return _context.ServiceRequests
                .Where(r => r.Email == customerEmail && r.CompletedDate != null)
                .ToList();
        }
    }
}
