using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using Services;
using Data.Models;

namespace Pages.Admin
{
    public class JobHistoryExportModel : PageModel
    {
        private readonly CustomerPortalService _portalService;
        private readonly ReceiptArchiveService _archiveService;
        public List<Data.Models.Customer> Customers { get; set; } = new();
        public List<ServiceRequest> Jobs { get; set; } = new();
        [BindProperty(SupportsGet = true)]
        public string CustomerEmail { get; set; } = string.Empty;
        [BindProperty(SupportsGet = true)]
        public string Date { get; set; } = string.Empty;
        public JobHistoryExportModel(CustomerPortalService portalService, ReceiptArchiveService archiveService)
        {
            _portalService = portalService;
            _archiveService = archiveService;
        }
        public void OnGet()
        {
            Customers = _portalService.GetCustomerList();
            if (!string.IsNullOrEmpty(CustomerEmail))
            {
                Jobs = _portalService.GetServiceRequests(CustomerEmail);
                if (!string.IsNullOrEmpty(Date))
                {
                    Jobs = Jobs.Where(j => j.RequestedAt.ToShortDateString() == Date).ToList();
                }
            }
        }
        public IActionResult OnPostReissue(int id)
        {
            var path = _archiveService.RegenerateAndArchiveReceiptAsync(id).GetAwaiter().GetResult();
            if (!string.IsNullOrEmpty(path))
            {
                TempData["ReissueSuccess"] = "Invoice packet reissued and archived.";
            }
            else
            {
                TempData["ReissueError"] = "Failed to reissue invoice packet.";
            }
            return RedirectToPage();
        }
    }
}
