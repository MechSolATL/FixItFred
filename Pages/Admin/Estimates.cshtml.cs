using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Data.Models;
using System.Collections.Generic;
using System.Linq;

namespace MVP_Core.Pages.Admin
{
    public class EstimatesModel : PageModel
    {
        public List<BillingInvoiceRecord> Estimates { get; set; } = new();

        public void OnGet()
        {
            // TODO: Replace with actual data source
            Estimates = new List<BillingInvoiceRecord>
            {
                new BillingInvoiceRecord { Id = 1, CustomerName = "Alice", CustomerEmail = "alice@example.com", AmountDue = 350.00M, Status = "Open" },
                new BillingInvoiceRecord { Id = 2, CustomerName = "Bob", CustomerEmail = "bob@example.com", AmountDue = 420.00M, Status = "Paid" }
            };
        }
    }
}
