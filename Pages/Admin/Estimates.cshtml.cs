using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Data.Models;
using System.Collections.Generic;
using System.Linq;
using System;

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
                new BillingInvoiceRecord { Id = Guid.NewGuid(), CustomerName = "Alice", CustomerEmail = "alice@example.com", AmountTotal = 350.00M, AmountPaid = 0, Status = "Open", InvoiceDate = DateTime.UtcNow },
                new BillingInvoiceRecord { Id = Guid.NewGuid(), CustomerName = "Bob", CustomerEmail = "bob@example.com", AmountTotal = 420.00M, AmountPaid = 420.00M, Status = "Paid", InvoiceDate = DateTime.UtcNow }
            };
        }
    }
}
