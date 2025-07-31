using Data.Models;
using Services;
using Services.Email;

namespace Controllers
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
            _ = _serviceRequestService.CreateServiceRequest(
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
