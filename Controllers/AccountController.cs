using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MVP_Core.Controllers
{
    public class AccountController(ILogger<AccountController> logger) : Controller
    {
        private readonly ILogger<AccountController> _logger = logger;

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            _logger.LogInformation("GET Login page accessed.");
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public IActionResult Login(string username, string email)
        {
            _logger.LogInformation("POST Login attempt with username: {Username}, email: {Email}", username, email);

            // Simulate login logic
            if (username == "testuser" && email == "testuser@example.com")
            {
                TempData["SuccessMessage"] = "Login successful!";
                _logger.LogInformation("Login successful for username: {Username}", username);
                return RedirectToAction("ServiceRequest", "Request");
            }
            else
            {
                TempData["ErrorMessage"] = "Invalid username or email.";
                _logger.LogWarning("Login failed for username: {Username}", username);
                return View();
            }
        }
    }
}
