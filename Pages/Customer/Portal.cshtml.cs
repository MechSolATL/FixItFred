using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Services;
using System.Collections.Generic;

namespace MVP_Core.Pages.Customer
{
    public class PortalModel : PageModel
    {
        private readonly CustomerPortalService _portalService;
        public MVP_Core.Data.Models.Customer? Customer { get; set; }
        public List<MVP_Core.Data.Models.ServiceRequest> Requests { get; set; } = new();
        public List<MVP_Core.Data.Models.LoyaltyPointTransaction> Loyalty { get; set; } = new();
        public MVP_Core.Data.Models.RewardTier? CurrentTier { get; set; }
        public PortalModel(CustomerPortalService portalService)
        {
            _portalService = portalService;
        }
        public void OnGet()
        {
            if (!User.Identity.IsAuthenticated || !User.HasClaim("IsCustomer", "true"))
            {
                return;
            }
            var email = User.Identity.Name ?? string.Empty;
            Requests = _portalService.GetServiceRequests(email);
            Loyalty = _portalService.GetLoyaltyTransactions(email);
            Customer = _portalService.GetCustomer(email);
            CurrentTier = _portalService.GetRewardTiers().LastOrDefault(t => Loyalty.Sum(l => l.Points) >= t.PointsRequired);
        }
    }
}
