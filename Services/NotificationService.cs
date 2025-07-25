using System.Threading.Tasks;
using MVP_Core.Data.Models;
namespace MVP_Core.Services
{
    public interface INotificationService
    {
        void SendEscalationAlert(ServiceRequest req);
        // Sprint 42.2 – Reply Alerts via Email/SMS
        Task SendEmailAsync(string toEmail, string subject, string htmlBody);
        Task SendSMSAsync(string toPhone, string message); // stub
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
    }
}
