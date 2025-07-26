using MVP_Core.Data;
using MVP_Core.Data.Models;
using MVP_Core.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace MVP_Core.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly LoyaltyRewardService _loyaltyService;
        public RegisterModel(ApplicationDbContext db, LoyaltyRewardService loyaltyService)
        {
            _db = db;
            _loyaltyService = loyaltyService;
        }
        [BindProperty]
        public string Name { get; set; } = string.Empty;
        [BindProperty]
        public string Email { get; set; } = string.Empty;
        [BindProperty]
        public string Phone { get; set; } = string.Empty;
        [BindProperty]
        public string Address { get; set; } = string.Empty;
        [BindProperty]
        public string? ReferralCodeInput { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Email))
            {
                ErrorMessage = "Name and Email are required.";
                return Page();
            }
            if (_db.Customers.Any(c => c.Email == Email))
            {
                ErrorMessage = "Email already registered.";
                return Page();
            }
            var customer = new MVP_Core.Data.Models.Customer { Name = Name, Email = Email, Phone = Phone, Address = Address };
            _db.Customers.Add(customer);
            await _db.SaveChangesAsync();
            // Referral logic
            if (!string.IsNullOrWhiteSpace(ReferralCodeInput))
            {
                var code = await _db.ReferralCodes.FirstOrDefaultAsync(r => r.Code == ReferralCodeInput && r.IsActive);
                if (code != null)
                {
                    // Anti-abuse: flag if same IP, device, or email domain
                    var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "";
                    var domain = Email.Split('@').LastOrDefault() ?? "";
                    var recent = await _db.ReferralEventLogs.Where(e => e.ReferralCodeId == code.Id && (e.Notes == ip || e.Notes == domain)).FirstOrDefaultAsync();
                    if (recent != null)
                    {
                        code.FraudFlagLevel = 1;
                        await _db.SaveChangesAsync();
                    }
                    // Log event
                    _db.ReferralEventLogs.Add(new ReferralEventLog {
                        ReferralCodeId = code.Id,
                        ReferrerCustomerId = code.OwnerCustomerId,
                        ReferredCustomerId = customer.Id,
                        EventType = "Signup",
                        Timestamp = DateTime.UtcNow,
                        Notes = ip + "," + domain
                    });
                    code.UsageCount++;
                    await _db.SaveChangesAsync();
                    // Reward both parties
                    _loyaltyService.AwardPoints(code.OwnerCustomerId, 25, "ReferralBonus", $"Referral bonus for referring {Name}");
                    _loyaltyService.AwardPoints(customer.Id, 25, "ReferralBonus", $"Referral bonus for signing up with code {code.Code}");
                }
            }
            return RedirectToPage("/Customer/Portal");
        }
    }
}
