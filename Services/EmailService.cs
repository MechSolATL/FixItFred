using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace MVP_Core.Services
{
    // ✅ Refactored with a primary constructor
    public class EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        private readonly string _apiKey = configuration["SendGrid:ApiKey"]
                                          ?? throw new ArgumentNullException(nameof(configuration), "SendGrid API key is missing.");
        private readonly ILogger<EmailService> _logger = logger;

        public async Task SendVerificationEmailAsync(string email, string verificationLink)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(verificationLink))
            {
                _logger.LogError("Email or Verification Link is missing.");
                throw new ArgumentException("Email and Verification Link must be provided.");
            }

            try
            {
                var client = new SendGridClient(_apiKey);
                var from = new EmailAddress("no-reply@service-atlanta.com", "Mechanical Solutions Atlanta");
                var subject = "Verify Your Email - Plumbing Service";
                var to = new EmailAddress(email);
                var plainTextContent = $"Click the link to verify your email: {verificationLink}";
                var htmlContent = $"<strong>Click <a href='{verificationLink}'>here</a> to verify your email.</strong>";

                var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                var response = await client.SendEmailAsync(msg);

                if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
                {
                    _logger.LogInformation("Verification email sent to {Email}.", email);
                }
                else
                {
                    var errorMsg = await response.Body.ReadAsStringAsync();
                    _logger.LogError("Failed to send email to {Email}. Status Code: {StatusCode}, Error: {ErrorMsg}",
                        email, response.StatusCode, errorMsg);

                    throw new Exception($"Failed to send email. Status Code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email to {Email}", email);
                throw;
            }
        }
    }
}
