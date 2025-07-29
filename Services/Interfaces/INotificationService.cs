namespace MVP_Core.Services.Interfaces {
    public interface INotificationService {
        Task SendAsync(string toUser, string message);
    }
}