namespace MVP_Core.Data.Models
{
    public class Customer
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;   // ✅ Initialized

        public string Email { get; set; } = string.Empty;  // ✅ Initialized
    }
}
