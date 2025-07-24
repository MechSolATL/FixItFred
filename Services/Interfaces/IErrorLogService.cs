namespace MVP_Core.Services.Interfaces
{
    public interface IErrorLogService
    {
        Task<IList<SessionError>> GetRecentErrorsAsync();
    }

    public class SessionError
    {
        public string UserName { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime TimeOccurred { get; set; }
    }
}
