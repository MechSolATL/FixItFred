using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using Data;
using Microsoft.EntityFrameworkCore;

namespace Controllers.Api
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
            {
                return false;
            }

            try
            {
                string code = GenerateCode();
                DateTime expiration = DateTime.UtcNow.AddMinutes(15);

                // Remove any existing record for this email
                var existing = await _db.EmailVerifications.FirstOrDefaultAsync(e => e.Email == email);
                if (existing != null)
                {
                    _db.EmailVerifications.Remove(existing);
                    await _db.SaveChangesAsync().ConfigureAwait(false);
                }

                var verification = new EmailVerification
                {
                    Email = email.Trim(),
                    Code = code,
                    Expiration = expiration,
                    IsVerified = false
                };

                _db.EmailVerifications.Add(verification);
                await _db.SaveChangesAsync().ConfigureAwait(false);

                return await SendEmailAsync(email, code).ConfigureAwait(false);
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
            {
                return false;
            }

            var record = await _db.EmailVerifications
                .FirstOrDefaultAsync(e => e.Email == email && !e.IsVerified);

            if (record == null || record.Expiration < DateTime.UtcNow)
            {
                _logger.LogWarning("Verification failed: record not found or expired for {Email}", email);
                return false;
            }

            if (!SecureEquals(record.Code.Trim(), submittedCode.Trim()))
            {
                return false;
            }

            record.IsVerified = true;
            await _db.SaveChangesAsync().ConfigureAwait(false);
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
                await _db.SaveChangesAsync().ConfigureAwait(false);
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
                string? host = smtp["Host"];
                int port = int.Parse(smtp["Port"] ?? "587");
                string? user = smtp["Username"];
                string? pass = smtp["Password"];
                string? from = smtp["FromEmail"];
                string? name = smtp["FromName"];
                bool enableSsl = bool.Parse(smtp["UseSSL"] ?? "true");

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

                await client.SendMailAsync(message).ConfigureAwait(false);
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
            byte[] bytes = new byte[4];
            RandomNumberGenerator.Fill(bytes);
            int value = BitConverter.ToInt32(bytes, 0) & 0x7FFFFFFF; // Ensure non-negative
            return (value % 900000 + 100000).ToString("D6");
        }

        private static bool SecureEquals(string a, string b)
        {
            if (a.Length != b.Length) return false;
            int result = 0;
            for (int i = 0; i < a.Length; i++)
                result |= a[i] ^ b[i];
            return result == 0;
        }
    }
}
