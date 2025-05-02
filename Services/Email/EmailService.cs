using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using MVP_Core.Data.Models;

namespace MVP_Core.Services.Email
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;
        private readonly string _serviceRequestFromEmail;
        private readonly string _noReplyFromEmail;
        private readonly string _adminNotificationEmail;
        private readonly string _fromName;
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;
        private readonly bool _useSsl;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;

            var smtpSection = _configuration.GetSection("SMTP");

            _smtpHost = smtpSection.GetValue<string>("Host") ?? throw new InvalidOperationException("SMTP Host missing.");
            _smtpPort = smtpSection.GetValue<int>("Port");
            _smtpUsername = smtpSection.GetValue<string>("Username") ?? throw new InvalidOperationException("SMTP Username missing.");
            _smtpPassword = smtpSection.GetValue<string>("Password") ?? throw new InvalidOperationException("SMTP Password missing.");
            _useSsl = smtpSection.GetValue<bool>("UseSSL");
            _fromName = smtpSection.GetValue<string>("FromName") ?? "Service-Atlanta";

            _serviceRequestFromEmail = smtpSection.GetValue<string>("FromEmail_ServiceRequest")
                ?? throw new InvalidOperationException("SMTP FromEmail_ServiceRequest missing.");

            _noReplyFromEmail = smtpSection.GetValue<string>("FromEmail_NoReply")
                ?? throw new InvalidOperationException("SMTP FromEmail_NoReply missing.");

            _adminNotificationEmail = smtpSection.GetValue<string>("AdminNotificationEmail")
                ?? throw new InvalidOperationException("SMTP AdminNotificationEmail missing.");
        }

        private async Task SendEmailAsync(string fromEmail, string toEmail, string subject, string plainTextContent, string htmlContent)
        {
            if (!IsValidEmail(toEmail))
            {
                _logger.LogError("Invalid email address: {Email}", toEmail);
                throw new ArgumentException("Invalid email address.");
            }

            using (var client = new SmtpClient(_smtpHost, _smtpPort))
            {
                client.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
                client.EnableSsl = _useSsl;

                using (var message = new MailMessage())
                {
                    message.From = new MailAddress(fromEmail, _fromName);
                    message.Subject = subject;
                    message.Body = htmlContent;
                    message.IsBodyHtml = true;
                    message.To.Add(toEmail);

                    try
                    {
                        await client.SendMailAsync(message);
                        _logger.LogInformation("Email sent to {Email}", toEmail);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error sending email to {Email}", toEmail);
                        throw;
                    }
                }
            }
        }

        public async Task SendVerificationEmailAsync(string email, string verificationLink)
        {
            var subject = "Verify Your Email - Service Atlanta";
            var plainText = $"Please verify your email: {verificationLink}";
            var html = $@"
                <div style='font-family:Poppins,sans-serif;line-height:1.6;'>
                    <p>Hello 👋,</p>
                    <p>Please verify your email by clicking:</p>
                    <p><a href='{verificationLink}' style='color:#007BFF;'>Verify My Email</a></p>
                    <p>If you didn't request this, no action needed. 🚀</p>
                    <p>- Service-Atlanta.com</p>
                </div>";

            await SendEmailAsync(_noReplyFromEmail, email, subject, plainText, html);
        }

        public async Task SendServiceRequestConfirmationToCustomerAsync(ServiceRequest request)
        {
            var subject = "Thank you for your Service Request!";
            var plainText = $"Hi {request.CustomerName},\n\nThanks for submitting your request. We'll be in touch soon.\nService Type: {request.ServiceType}\nDetails: {request.Details}";
            var html = $@"
                <div style='font-family:Arial,sans-serif;line-height:1.5;'>
                    <h2>Hello {request.CustomerName},</h2>
                    <p>Thanks for submitting your service request! Here's what we received:</p>
                    <ul>
                        <li><strong>Service:</strong> {request.ServiceType}</li>
                        <li><strong>Details:</strong> {request.Details}</li>
                        <li><strong>Date:</strong> {request.CreatedAt:MMMM dd, yyyy}</li>
                    </ul>
                    <p>We will be contacting you shortly!<br>– Mechanical Solutions Atlanta</p>
                </div>";

            await SendEmailAsync(_serviceRequestFromEmail, request.Email, subject, plainText, html);
        }

        public async Task NotifyAdminOfNewRequestAsync(ServiceRequest request)
        {
            var subject = "🛎️ New Service Request Received!";
            var plainText = $"New request from {request.CustomerName}.";
            var html = $@"
                <div style='font-family:Arial,sans-serif;line-height:1.5;'>
                    <h2>New Service Request Submitted</h2>
                    <ul>
                        <li><strong>Name:</strong> {request.CustomerName}</li>
                        <li><strong>Phone:</strong> {request.Phone}</li>
                        <li><strong>Email:</strong> {request.Email}</li>
                        <li><strong>Service:</strong> {request.ServiceType}</li>
                        <li><strong>Details:</strong> {request.Details}</li>
                        <li><strong>Date:</strong> {request.CreatedAt:MMMM dd, yyyy}</li>
                    </ul>
                </div>";

            await SendEmailAsync(_serviceRequestFromEmail, _adminNotificationEmail, subject, plainText, html);
        }

        private static bool IsValidEmail(string email)
        {
            var emailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return !string.IsNullOrWhiteSpace(email) && Regex.IsMatch(email, emailRegex);
        }
    }
}
