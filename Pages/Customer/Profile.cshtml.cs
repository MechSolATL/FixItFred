using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Data;
using Services;

namespace Pages.Customer
{
    public class ProfileModel : PageModel
    {
        private readonly CustomerPortalService _portalService; // Sprint78.7-ProfileNullSafetyPatch: Constructor injection
        private readonly ApplicationDbContext _db; // Sprint78.7-ProfileNullSafetyPatch: Constructor injection
        public Data.Models.Customer? Customer { get; set; } // Sprint78.7-ProfileNullSafetyPatch: Nullable property
        public ProfileModel(CustomerPortalService portalService, ApplicationDbContext db)
        {
            _portalService = portalService ?? throw new ArgumentNullException(nameof(portalService)); // Sprint78.7-ProfileNullSafetyPatch: Null guard
            _db = db ?? throw new ArgumentNullException(nameof(db)); // Sprint78.7-ProfileNullSafetyPatch: Null guard
        }
        public void OnGet()
        {
            if (!(User?.Identity?.IsAuthenticated ?? false) || !(User?.HasClaim("IsCustomer", "true") ?? false)) // Sprint78.7-ProfileNullSafetyPatch: Null-safe user check
            {
                return;
            }
            var email = User?.Identity?.Name ?? string.Empty; // Sprint78.7-ProfileNullSafetyPatch: Null-safe fallback
            Customer = _portalService.GetCustomer(email); // Sprint78.7-ProfileNullSafetyPatch: Null-safe service access
        }
        public IActionResult OnPost()
        {
            if (!(User?.Identity?.IsAuthenticated ?? false) || !(User?.HasClaim("IsCustomer", "true") ?? false)) // Sprint78.7-ProfileNullSafetyPatch: Null-safe user check
            {
                return RedirectToPage();
            }
            var email = User?.Identity?.Name ?? string.Empty; // Sprint78.7-ProfileNullSafetyPatch: Null-safe fallback
            var customer = _db.Customers.FirstOrDefault(c => c.Email == email); // Sprint78.7-ProfileNullSafetyPatch: Null-safe db access
            if (customer != null)
            {
                customer.Name = Request?.Form["Name"].ToString() ?? customer.Name; // Sprint78.7-ProfileNullSafetyPatch: Null-safe form access
                customer.Email = Request?.Form["Email"].ToString() ?? customer.Email; // Sprint78.7-ProfileNullSafetyPatch: Null-safe form access
                customer.Phone = Request?.Form["Phone"].ToString() ?? customer.Phone; // Sprint78.7-ProfileNullSafetyPatch: Null-safe form access
                customer.Address = Request?.Form["Address"].ToString() ?? customer.Address; // Sprint78.7-ProfileNullSafetyPatch: Null-safe form access
                _db.SaveChanges(); // Sprint78.7-ProfileNullSafetyPatch: Null-safe db access
            }
            return RedirectToPage();
        }
    }
}
