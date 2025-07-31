using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using Services;
using Data.Models;

namespace Pages.Customer
{
    public class DisputeCenterModel : PageModel
    {
        private readonly DisputeService _disputeService; // Sprint 78.6: Constructor injection
        private readonly ServiceRequestService _serviceRequestService; // Sprint 78.6: Constructor injection
        public List<ServiceRequest> CompletedRequests { get; set; } = new(); // Sprint 78.6: Initialized to resolve CS8618
        public List<DisputeRecord> MyDisputes { get; set; } = new(); // Sprint 78.6: Initialized to resolve CS8618
        public bool SubmitSuccess { get; set; } = false; // Sprint 78.6: Initialized to resolve CS8618
        public DisputeCenterModel(DisputeService disputeService, ServiceRequestService serviceRequestService)
        {
            _disputeService = disputeService ?? throw new ArgumentNullException(nameof(disputeService)); // Sprint 78.6: Constructor null guard
            _serviceRequestService = serviceRequestService ?? throw new ArgumentNullException(nameof(serviceRequestService)); // Sprint 78.6: Constructor null guard
        }
        public void OnGet()
        {
            var userId = User?.Identity?.Name ?? "anonymous"; // Sprint 78.6: Null-safe fallback
            CompletedRequests = _serviceRequestService.GetCompletedRequestsByCustomer(userId); // Sprint 78.6: Null-safe service access
            MyDisputes = _disputeService.GetDisputesByCustomer(userId); // Sprint 78.6: Null-safe service access
        }
        public IActionResult OnPost()
        {
            var userId = User?.Identity?.Name ?? "anonymous"; // Sprint 78.6: Null-safe fallback
            int.TryParse(Request?.Form["ServiceRequestId"].ToString(), out var serviceRequestId); // Sprint 78.6: Null-safe form access
            var reason = Request?.Form["Reason"].ToString() ?? string.Empty; // Sprint 78.6: Null-safe form access
            var details = Request?.Form["Details"].ToString() ?? string.Empty; // Sprint 78.6: Null-safe form access
            var file = Request?.Form?.Files?["Attachment"]; // Sprint 78.6: Null-safe form access
            string? attachmentPath = null;
            if (file != null && file.Length > 0)
            {
                var fileName = $"dispute_{userId}_{DateTime.UtcNow:yyyyMMddHHmmss}_{file.FileName}"; // Sprint 78.6
                var savePath = $"wwwroot/uploads/disputes/{fileName}"; // Sprint 78.6
                using (var stream = System.IO.File.Create(savePath)) // Sprint 78.6
                {
                    file.CopyTo(stream); // Sprint 78.6
                }
                attachmentPath = $"/uploads/disputes/{fileName}"; // Sprint 78.6
            }
            var dispute = new DisputeRecord
            {
                CustomerEmail = userId, // Sprint 78.6
                ServiceRequestId = serviceRequestId, // Sprint 78.6
                Reason = reason, // Sprint 78.6
                Details = details, // Sprint 78.6
                AttachmentPath = attachmentPath // Sprint 78.6
            };
            _disputeService.SubmitDispute(dispute); // Sprint 78.6: Null-safe service access
            SubmitSuccess = true; // Sprint 78.6
            OnGet(); // Sprint 78.6
            return Page(); // Sprint 78.6
        }
    }
}
