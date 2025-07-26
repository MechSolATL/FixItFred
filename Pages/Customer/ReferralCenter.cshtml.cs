using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Data;
using MVP_Core.Data.Models;
using MVP_Core.Services;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Collections.Generic;

namespace MVP_Core.Pages.Customer
{
    public class ReferralCenterModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly LoyaltyRewardService _loyaltyService;
        public ReferralCenterModel(ApplicationDbContext db, LoyaltyRewardService loyaltyService)
        {
            _db = db;
            _loyaltyService = loyaltyService;
        }
        public ReferralCode? ReferralCode { get; set; }
        public List<ReferralEventLog> UsageLogs { get; set; } = new();
        public List<LoyaltyPointTransaction> Rewards { get; set; } = new();
        public void OnGet()
        {
            var email = User.Identity?.Name ?? string.Empty;
            var customer = _db.Customers.FirstOrDefault(c => c.Email == email);
            if (customer != null)
            {
                ReferralCode = _db.ReferralCodes.FirstOrDefault(r => r.OwnerCustomerId == customer.Id && r.IsActive);
                if (ReferralCode != null)
                {
                    UsageLogs = _db.ReferralEventLogs.Where(e => e.ReferralCodeId == ReferralCode.Id).OrderByDescending(e => e.Timestamp).ToList();
                    Rewards = _loyaltyService.GetTransactions(customer.Id).Where(t => t.Type.Contains("Referral")).ToList();
                }
            }
        }
    }
}
