using Microsoft.AspNetCore.Mvc;
using MVP_Core.Data.Models;
using MVP_Core.Services.Email;

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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ServiceRequest(ServiceRequest model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Save the service request
            var requestId = await _serviceRequestService.CreateServiceRequestAsync(
                model.CustomerName,
                model.Email,
                model.Phone,
                model.Address,
                model.ServiceType,
                model.ServiceSubtype,
                model.Details,
                model.SessionId,
                model.IsUrgent
            );

            // Send email to customer and admin
            await _emailService.SendServiceRequestConfirmationToCustomerAsync(model);
            await _emailService.NotifyAdminOfNewRequestAsync(model);

            return RedirectToAction("Confirmation");
        }

        [HttpGet]
        public IActionResult Confirmation()
        {
            return View();
        }
    }
}
