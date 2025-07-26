using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace MVP_Core.Pages.Customer
{
    public class ProfileModel : PageModel
    {
        private readonly CustomerPortalService _portalService;
        private readonly ApplicationDbContext _db;
        public MVP_Core.Data.Models.Customer? Customer { get; set; }
        public ProfileModel(CustomerPortalService portalService, ApplicationDbContext db)
        {
            _portalService = portalService;
            _db = db;
        }
        public void OnGet()
        {
            if (!User.Identity.IsAuthenticated || !User.HasClaim("IsCustomer", "true"))
            {
                return;
            }
            var email = User.Identity.Name ?? string.Empty;
            Customer = _portalService.GetCustomer(email);
        }
        public IActionResult OnPost()
        {
            if (!User.Identity.IsAuthenticated || !User.HasClaim("IsCustomer", "true"))
            {
                return RedirectToPage();
            }
            var email = User.Identity.Name ?? string.Empty;
            var customer = _db.Customers.FirstOrDefault(c => c.Email == email);
            if (customer != null)
            {
                customer.Name = Request.Form["Name"];
                customer.Email = Request.Form["Email"];
                customer.Phone = Request.Form["Phone"];
                customer.Address = Request.Form["Address"];
                _db.SaveChanges();
            }
            return RedirectToPage();
        }
    }
}
