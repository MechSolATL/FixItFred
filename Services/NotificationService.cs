using MVP_Core.Data.Models;
namespace MVP_Core.Services
{
    public interface INotificationService
    {
        void SendEscalationAlert(ServiceRequest req);
    }
    public class NotificationService : INotificationService
    {
        public void SendEscalationAlert(ServiceRequest req)
        {
            // TODO: Implement email/SMS logic
        }
    }
}
