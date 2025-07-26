using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Services;
using MVP_Core.Data.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;

namespace MVP_Core.Pages.Customer
{
    public class DisputeCenterModel : PageModel
    {
        private readonly DisputeService _disputeService;
        private readonly ServiceRequestService _serviceRequestService;
        public List<ServiceRequest> CompletedRequests { get; set; } = new();
        public List<DisputeRecord> MyDisputes { get; set; } = new();
        public bool SubmitSuccess { get; set; } = false;
        public DisputeCenterModel(DisputeService disputeService, ServiceRequestService serviceRequestService)
        {
            _disputeService = disputeService;
            _serviceRequestService = serviceRequestService;
        }
        public void OnGet()
        {
            var email = User?.Identity?.Name ?? "";
            CompletedRequests = _serviceRequestService.GetCompletedRequestsByCustomer(email);
            MyDisputes = _disputeService.GetDisputesByCustomer(email);
        }
        public IActionResult OnPost()
        {
            var email = User?.Identity?.Name ?? "";
            int.TryParse(Request.Form["ServiceRequestId"], out var serviceRequestId);
            var reason = Request.Form["Reason"];
            var details = Request.Form["Details"];
            var file = Request.Form.Files["Attachment"];
            string? attachmentPath = null;
            if (file != null && file.Length > 0)
            {
                var fileName = $"dispute_{email}_{DateTime.UtcNow:yyyyMMddHHmmss}_{file.FileName}";
                var savePath = $"wwwroot/uploads/disputes/{fileName}";
                using (var stream = System.IO.File.Create(savePath))
                {
                    file.CopyTo(stream);
                }
                attachmentPath = $"/uploads/disputes/{fileName}";
            }
            var dispute = new DisputeRecord
            {
                CustomerEmail = email,
                ServiceRequestId = serviceRequestId,
                Reason = reason,
                Details = details,
                AttachmentPath = attachmentPath
            };
            _disputeService.SubmitDispute(dispute);
            SubmitSuccess = true;
            OnGet();
            return Page();
        }
    }
}
