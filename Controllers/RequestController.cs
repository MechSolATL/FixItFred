using Microsoft.AspNetCore.Mvc;
using MVP_Core.Data.Models;
using MVP_Core.Services;

namespace MVP_Core.Controllers
{
    public class RequestController(ServiceRequestService serviceRequestService, EmailService emailService) : Controller
    {
        private readonly ServiceRequestService _serviceRequestService = serviceRequestService;
        private readonly EmailService _emailService = emailService;

        [HttpGet]
        public IActionResult ServiceRequest()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ServiceRequest(ServiceRequest model)
        {
            if (!ModelState.IsValid)
            {
                return View(model); // Return form with validation errors
            }

            // Save the service request
            model.CreatedAt = DateTime.UtcNow;
            model.Status = "Pending";
            await _serviceRequestService.SaveRequestAsync(model);

            // Send email to user and admin
            await _emailService.SendServiceRequestConfirmationEmailAsync(model.Email, model);
            await _emailService.NotifyAdminOfNewRequest(model);

            return RedirectToAction("Confirmation");
        }

        [HttpGet]
        public IActionResult Confirmation()
        {
            return View();
        }
    }
}
