namespace Services.Interfaces
{
    public interface INotificationService {
        Task SendAsync(string toUser, string message);
    }
}