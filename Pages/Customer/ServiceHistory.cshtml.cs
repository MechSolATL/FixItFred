using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Services;
using MVP_Core.Data.Models;
using System.Collections.Generic;

namespace MVP_Core.Pages.Customer
{
    public class ServiceHistoryModel : PageModel
    {
        private readonly CustomerPortalService _portalService;
        public List<ServiceRequest> Requests { get; set; } = new();
        public void OnGet()
        {
            if (!User.Identity.IsAuthenticated || !User.HasClaim("IsCustomer", "true"))
            {
                return;
            }
            var email = User.Identity.Name ?? string.Empty;
            Requests = _portalService.GetServiceRequests(email);
        }
    }
}
