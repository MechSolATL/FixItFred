using System.Threading.Tasks;
using Data.Models;

namespace Services
{
    public interface INotificationService
    {
        void SendEscalationAlert(ServiceRequest req);
        // Sprint 42.2 – Reply Alerts via Email/SMS
        Task SendEmailAsync(string toEmail, string subject, string htmlBody);
        Task SendSMSAsync(string toPhone, string message); // stub
        Task SendAsync(object recipient, string message);
        void Notify(string message); // Added for ExecutiveDigest
    }
    public class NotificationService : INotificationService
    {
        public void SendEscalationAlert(ServiceRequest req)
        {
            // TODO: Implement email/SMS logic
        }
        // Sprint 42.2 – Reply Alerts via Email/SMS
        public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            // TODO: Implement using SMTP/SendGrid or inject EmailService
            await Task.CompletedTask;
        }
        public async Task SendSMSAsync(string toPhone, string message)
        {
            // TODO: Implement SMS sending (stub)
            await Task.CompletedTask;
        }
        public async Task SendAsync(object recipient, string message)
        {
            // Sprint 86.7 — Unified send for SMS/email/alerts
            if (recipient is int customerId)
            {
                // Lookup customer email/phone by ID (stub)
                await SendEmailAsync($"customer{customerId}@example.com", "Notification", message);
            }
            else if (recipient is string emailOrPhone && emailOrPhone.Contains("@"))
            {
                await SendEmailAsync(emailOrPhone, "Notification", message);
            }
            else if (recipient is string phone)
            {
                await SendSMSAsync(phone, message);
            }
        }
        public void Notify(string message)
        {
            // Stub for ExecutiveDigest notification
        }
    }
}
