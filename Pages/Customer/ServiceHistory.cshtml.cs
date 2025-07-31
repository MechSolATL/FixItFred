using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Services;
using Data.Models;

namespace Pages.Customer
{
    public class ServiceHistoryModel : PageModel
    {
        private readonly CustomerPortalService _portalService;
        private readonly ReceiptArchiveService _archiveService;
        public List<ServiceRequest> Requests { get; set; } = new();
        public List<Data.Models.Technician> Technicians { get; set; } = new();
        public ServiceHistoryModel(CustomerPortalService portalService, ReceiptArchiveService archiveService)
        {
            _portalService = portalService;
            _archiveService = archiveService;
        }
        public void OnGet()
        {
            if (!User.Identity.IsAuthenticated || !User.HasClaim("IsCustomer", "true"))
            {
                return;
            }
            var email = User.Identity.Name ?? string.Empty;
            Requests = _portalService.GetServiceRequests(email).Where(r => r.ShowInTimeline).OrderByDescending(r => r.RequestedAt).ToList();
            Technicians = _portalService.GetTechnicians();
        }
        public Data.Models.Technician? GetTechnicianById(int? id)
        {
            if (!id.HasValue) return null;
            return Technicians.FirstOrDefault(t => t.Id == id.Value);
        }
    }
}
