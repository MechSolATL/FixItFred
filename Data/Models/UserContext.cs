namespace Data.Models
{
    public class UserContext
    {
        public string UserName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsAuthenticated { get; set; } = false;
    }
}
