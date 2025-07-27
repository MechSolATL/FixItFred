using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Services;
using BillingInvoiceRecordModel = MVP_Core.Data.Models.BillingInvoiceRecord;
using System.Collections.Generic;

namespace MVP_Core.Pages.Customer
{
    public class DocumentsModel : PageModel
    {
        private readonly CustomerPortalService _portalService; // existing line
        public DocumentsModel(CustomerPortalService portalService)
        {
            _portalService = portalService ?? throw new ArgumentNullException(nameof(portalService)); // Sprint 78.5: Injected to resolve CS0649
        }
        public List<BillingInvoiceRecordModel> Documents { get; set; } = new(); // Sprint 78.4: Razor/backend context sync for property access
        public bool ShowUploads { get; set; } = false; // Sprint 78.4: Razor/backend context sync for property access
        public bool? IsVerified { get; set; } = null; // Sprint 78.4: Razor/backend context sync for property access
        public void OnGet()
        {
            // Sprint 78.4: Razor/backend context sync for property access
            if (this.ShowUploads || (this.IsVerified ?? false))
            {
                if (!User?.Identity?.IsAuthenticated ?? false || !User?.HasClaim("IsCustomer", "true") ?? false)
                {
                    return;
                }
                var email = User?.Identity?.Name ?? string.Empty; // Sprint 78.2: Hardened for CS860X
                if (_portalService != null) // Sprint 78.2: Hardened for CS860X
                {
                    this.Documents = _portalService.GetDocuments(email); // Sprint 78.4: Razor/backend context sync for property access
                }
            }
        }
    }
}
