using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using MVP_Core.Data.Models;

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

        /// <summary>
        /// Sends a generic email using SendGrid.
        /// </summary>
        private async Task<bool> SendEmailAsync(string toEmail, string subject, string plainTextContent, string htmlContent, int retryCount = 3)
        {
            if (!IsValidEmail(toEmail))
            {
                _logger.LogError("Invalid email address: {Email}", toEmail);
                throw new ArgumentException("Invalid email address.");
            }

            for (int attempt = 0; attempt < retryCount; attempt++)
            {
                try
                {
                    var client = new SendGridClient(_apiKey);
                    var from = new EmailAddress("no-reply@service-atlanta.com", "Mechanical Solutions Atlanta");
                    var to = new EmailAddress(toEmail);
                    var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

                    var response = await client.SendEmailAsync(msg);

                    if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
                    {
                        _logger.LogInformation("Email successfully sent to {Email}.", toEmail);
                        return true;
                    }

                    var errorMsg = await response.Body.ReadAsStringAsync();
                    _logger.LogError("Failed to send email to {Email}. Status Code: {StatusCode}, Error: {ErrorMsg}",
                        toEmail, response.StatusCode, errorMsg);

                    throw new Exception($"Failed to send email. Status Code: {response.StatusCode}");
                }
                catch (Exception ex)
                {
                    if (attempt == retryCount - 1)
                    {
                        _logger.LogError(ex, "Failed to send email to {Email} after {RetryCount} attempts.", toEmail, retryCount);
                        throw;
                    }
                    _logger.LogWarning("Retrying email to {Email}. Attempt {Attempt} of {RetryCount}.", toEmail, attempt + 1, retryCount);
                    await Task.Delay(TimeSpan.FromSeconds(2)); // Exponential backoff could be added here
                }
            }

            return false;
        }

        /// <summary>
        /// Sends a verification email to a user.
        /// </summary>
        public async Task SendVerificationEmailAsync(string email, string verificationLink)
        {
            var subject = "Verify Your Email - Plumbing Service";
            var plainTextContent = $"Click the link to verify your email: {verificationLink}";
            var htmlContent = $@"
                <div style='font-family:Arial,sans-serif;line-height:1.5;'>
                    <p>Click the link below to verify your email:</p>
                    <p><a href='{verificationLink}' style='color:#007BFF;'>Verify Email</a></p>
                    <p>If you did not request this, please ignore this email.</p>
                    <p>Thank you,<br>Mechanical Solutions Atlanta</p>
                </div>";

            await SendEmailAsync(email, subject, plainTextContent, htmlContent);
        }

        /// <summary>
        /// Sends a service request confirmation email to the user.
        /// </summary>
        public async Task SendServiceRequestConfirmationEmailAsync(string email, ServiceRequest request)
        {
            var subject = "Service Request Confirmation";
            var plainTextContent = $@"
                Dear {request.CustomerName},

                Thank you for submitting your service request. Here are the details:
                - Service Type: {request.ServiceType}
                - Details: {request.Details}
                - Date: {request.CreatedAt:MMMM dd, yyyy}

                We will contact you shortly to confirm the next steps.
                Thank you,
                Mechanical Solutions Atlanta";
            var htmlContent = $@"
                <div style='font-family:Arial,sans-serif;line-height:1.5;'>
                    <p>Dear {request.CustomerName},</p>
                    <p>Thank you for submitting your service request. Here are the details:</p>
                    <ul>
                        <li><strong>Service Type:</strong> {request.ServiceType}</li>
                        <li><strong>Details:</strong> {request.Details}</li>
                        <li><strong>Date:</strong> {request.CreatedAt:MMMM dd, yyyy}</li>
                    </ul>
                    <p>We will contact you shortly to confirm the next steps.</p>
                    <p>Thank you,<br>Mechanical Solutions Atlanta</p>
                </div>";

            await SendEmailAsync(email, subject, plainTextContent, htmlContent);
        }

        /// <summary>
        /// Sends a notification email to the admin about a new service request.
        /// </summary>
        public async Task NotifyAdminOfNewRequest(ServiceRequest request)
        {
            var adminEmail = "admin@service-atlanta.com"; // Replace with actual admin email
            var subject = "New Service Request Submitted";
            var plainTextContent = $@"
                A new service request has been submitted:
                - Customer Name: {request.CustomerName}
                - Email: {request.Email}
                - Service Type: {request.ServiceType}
                - Details: {request.Details}
                - Date: {request.CreatedAt:MMMM dd, yyyy}";

            var htmlContent = $@"
                <div style='font-family:Arial,sans-serif;line-height:1.5;'>
                    <p>A new service request has been submitted:</p>
                    <ul>
                        <li><strong>Customer Name:</strong> {request.CustomerName}</li>
                        <li><strong>Email:</strong> {request.Email}</li>
                        <li><strong>Service Type:</strong> {request.ServiceType}</li>
                        <li><strong>Details:</strong> {request.Details}</li>
                        <li><strong>Date:</strong> {request.CreatedAt:MMMM dd, yyyy}</li>
                    </ul>
                </div>";

            await SendEmailAsync(adminEmail, subject, plainTextContent, htmlContent);
        }

        /// <summary>
        /// Validates the format of an email address.
        /// </summary>
        private static bool IsValidEmail(string email)
        {
            var emailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return !string.IsNullOrWhiteSpace(email) && Regex.IsMatch(email, emailRegex);
        }
    }
}
