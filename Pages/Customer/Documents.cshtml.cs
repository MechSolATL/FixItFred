using Microsoft.AspNetCore.Mvc.RazorPages;
using BillingInvoiceRecordModel = Data.Models.BillingInvoiceRecord;
using System.Collections.Generic;
using Services;

namespace Pages.Customer
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
        // Sprint 83.4-FinalFix: Migration blocker resolution.
        public void OnGet()
        {
            // Sprint 83.4-FinalFix: Migration blocker resolution.
            if ((IsVerified ?? false) || ShowUploads)
            {
                if (!(User?.Identity?.IsAuthenticated ?? false) || !(User?.HasClaim("IsCustomer", "true") ?? false))
                {
                    return;
                }
                var email = User?.Identity?.Name ?? string.Empty;
                if (_portalService != null)
                {
                    Documents = _portalService.GetDocuments(email);
                }
            }
        }
        // Sprint83.4-MigrationBlockerFixes
    }
}
