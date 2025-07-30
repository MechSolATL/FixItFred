namespace MVP_Core.Data.Models
{
    public class Customer
    {
        /// <summary>
        /// The unique identifier for the customer.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The name of the customer.
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The email address of the customer.
        /// </summary>
        [EmailAddress]
        public string? Email { get; set; }

        /// <summary>
        /// The phone number of the customer.
        /// </summary>
        [Phone]
        public string? Phone { get; set; }

        /// <summary>
        /// The address of the customer.
        /// </summary>
        [MaxLength(200)]
        public string? Address { get; set; }
    }
}
