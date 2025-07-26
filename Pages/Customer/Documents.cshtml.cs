using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Services;
using BillingInvoiceRecordModel = MVP_Core.Data.Models.BillingInvoiceRecord;
using System.Collections.Generic;

namespace MVP_Core.Pages.Customer
{
    public class DocumentsModel : PageModel
    {
        private readonly CustomerPortalService _portalService;
        public List<BillingInvoiceRecordModel> Documents { get; set; } = new();
        public void OnGet()
        {
            if (!User.Identity.IsAuthenticated || !User.HasClaim("IsCustomer", "true"))
            {
                return;
            }
            var email = User.Identity.Name ?? string.Empty;
            Documents = _portalService.GetDocuments(email);
        }
    }
}
