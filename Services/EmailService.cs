using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace MVP_Core.Services
{
    public class EmailService
    {
        private readonly string _apiKey;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _logger = logger;

            _apiKey = configuration["SendGrid:ApiKey"]!;
            if (string.IsNullOrEmpty(_apiKey))
            {
                _logger.LogError("SendGrid API key is not configured in appsettings.json.");
                throw new ArgumentNullException(nameof(configuration), "SendGrid API key is missing.");
            }
        }

        public async Task SendVerificationEmailAsync(string email, string verificationLink)
        {
            if (!IsValidEmail(email) || string.IsNullOrWhiteSpace(verificationLink))
            {
                _logger.LogError("Invalid email or verification link. Email: {Email}, Link: {Link}", email, verificationLink);
                throw new ArgumentException("Valid email and verification link must be provided.");
            }

            int retryCount = 3;
            for (int attempt = 0; attempt < retryCount; attempt++)
            {
                try
                {
                    var client = new SendGridClient(_apiKey);
                    var from = new EmailAddress("no-reply@service-atlanta.com", "Mechanical Solutions Atlanta");
                    var subject = "Verify Your Email - Plumbing Service";
                    var to = new EmailAddress(email);
                    var plainTextContent = $"Click the link to verify your email: {verificationLink}";
                    var htmlContent = $@"
                        <div style='font-family:Arial,sans-serif;line-height:1.5;'>
                            <p>Click the link below to verify your email:</p>
                            <p><a href='{verificationLink}' style='color:#007BFF;'>Verify Email</a></p>
                            <p>If you did not request this, please ignore this email.</p>
                            <p>Thank you,<br>Mechanical Solutions Atlanta</p>
                        </div>";

                    var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                    var response = await client.SendEmailAsync(msg);

                    if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
                    {
                        _logger.LogInformation("Verification email successfully sent to {Email}.", email);
                        return;
                    }

                    var errorMsg = await response.Body.ReadAsStringAsync();
                    _logger.LogError("Failed to send email to {Email}. Status Code: {StatusCode}, Error: {ErrorMsg}",
                        email, response.StatusCode, errorMsg);

                    throw new Exception($"Failed to send email. Status Code: {response.StatusCode}");
                }
                catch (Exception ex)
                {
                    if (attempt == retryCount - 1)
                    {
                        _logger.LogError(ex, "Failed to send email to {Email} after {RetryCount} attempts.", email, retryCount);
                        throw;
                    }
                    _logger.LogWarning("Retrying email to {Email}. Attempt {Attempt} of {RetryCount}.", email, attempt + 1, retryCount);
                    await Task.Delay(TimeSpan.FromSeconds(2));
                }
            }
        }

        private static bool IsValidEmail(string email)
        {
            return !string.IsNullOrWhiteSpace(email) && email.Contains('@') && email.Contains('.');
        }
    }
}
