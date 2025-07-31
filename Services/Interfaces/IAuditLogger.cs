namespace Services.Interfaces
{
    public interface IAuditLogger {
        Task LogAsync(string action, string user);
    }
}