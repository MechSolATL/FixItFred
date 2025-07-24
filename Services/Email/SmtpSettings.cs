namespace MVP_Core.Services.Email
{
    public class SmtpSettings
    {
        public required string Host { get; set; }
        public int Port { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string FromAddress { get; set; }
        public required string FromName { get; set; }
        public bool UseSSL { get; set; }
    }
}
