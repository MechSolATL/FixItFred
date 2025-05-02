using MVP_Core.Data;
using MVP_Core.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;

namespace MVP_Core.Controllers.Api
{
    /// <summary>
    /// Handles the generation, storage, and delivery of email verification codes.
    /// </summary>
    public class EmailVerificationService
    {
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _config;
        private readonly ILogger<EmailVerificationService> _logger;

        public EmailVerificationService(ApplicationDbContext db, IConfiguration config, ILogger<EmailVerificationService> logger)
        {
            _db = db;
            _config = config;
            _logger = logger;
        }

        /// <summary>
        /// Generates a 6-digit code, saves it, and emails it to the user.
        /// </summary>
        public async Task<bool> GenerateAndSendCodeAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
                return false;

            try
            {
                var code = GenerateCode();
                var expiration = DateTime.UtcNow.AddMinutes(15);

                // Remove any existing record for this email
                var existing = await _db.EmailVerifications.FirstOrDefaultAsync(e => e.Email == email);
                if (existing != null)
                {
                    _db.EmailVerifications.Remove(existing);
                    await _db.SaveChangesAsync();
                }

                var verification = new EmailVerification
                {
                    Email = email.Trim(),
                    Code = code,
                    Expiration = expiration,
                    IsVerified = false
                };

                _db.EmailVerifications.Add(verification);
                await _db.SaveChangesAsync();

                return await SendEmailAsync(email, code);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during code generation or sending.");
                return false;
            }
        }

        /// <summary>
        /// Confirms the code matches the saved entry and hasn't expired.
        /// </summary>
        public async Task<bool> VerifyCodeAsync(string email, string submittedCode)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(submittedCode))
                return false;

            var record = await _db.EmailVerifications
                .FirstOrDefaultAsync(e => e.Email == email && !e.IsVerified);

            if (record == null || record.Expiration < DateTime.UtcNow)
                return false;

            if (record.Code.Trim() != submittedCode.Trim())
                return false;

            record.IsVerified = true;
            await _db.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Deletes expired, unused verification entries.
        /// </summary>
        public async Task CleanExpiredCodesAsync()
        {
            var expired = await _db.EmailVerifications
                .Where(e => !e.IsVerified && e.Expiration < DateTime.UtcNow)
                .ToListAsync();

            if (expired.Any())
            {
                _db.EmailVerifications.RemoveRange(expired);
                await _db.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Sends the verification code via SMTP.
        /// </summary>
        private async Task<bool> SendEmailAsync(string toEmail, string code)
        {
            try
            {
                var smtp = _config.GetSection("SMTP");
                var host = smtp["Host"];
                var port = int.Parse(smtp["Port"] ?? "587");
                var user = smtp["Username"];
                var pass = smtp["Password"];
                var from = smtp["FromEmail_NoReply"];
                var name = smtp["FromName"];
                var enableSsl = bool.Parse(smtp["UseSSL"] ?? "true");

                var message = new MailMessage
                {
                    From = new MailAddress(from ?? throw new InvalidOperationException("SMTP FromEmail missing"), name),
                    Subject = "Your Verification Code",
                    Body = $"Your verification code is: {code}\n\nThis code expires in 15 minutes.",
                    IsBodyHtml = false
                };

                message.To.Add(toEmail);

                using var client = new SmtpClient(host, port)
                {
                    Credentials = new NetworkCredential(user, pass),
                    EnableSsl = enableSsl
                };

                await client.SendMailAsync(message);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send verification email to {toEmail}");
                return false;
            }
        }

        /// <summary>
        /// Generates a secure 6-digit numeric code.
        /// </summary>
        private string GenerateCode()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }
    }
}
