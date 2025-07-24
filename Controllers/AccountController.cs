namespace MVP_Core.Controllers
{
    public class AccountController(ILogger<AccountController> logger) : Controller
    {
        private readonly ILogger<AccountController> _logger = logger;

        [HttpGet]
        public IActionResult Login()
        {
            _logger.LogInformation("GET Login page accessed.");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Login POST invalid model state.");
                return View(model);
            }

            _logger.LogInformation("POST Login attempt with username: {Username}, email: {Email}", model.Username, model.Email);

            // Simulate login logic
            if (model.Username == "testuser" && model.Email == "testuser@example.com")
            {
                TempData["SuccessMessage"] = "Login successful!";
                _logger.LogInformation("Login successful for username: {Username}", model.Username);
                return RedirectToAction("ServiceRequest", "Request");
            }
            else
            {
                TempData["ErrorMessage"] = "Invalid username or email.";
                _logger.LogWarning("Login failed for username: {Username}", model.Username);
                return View(model);
            }
        }
    }
}
