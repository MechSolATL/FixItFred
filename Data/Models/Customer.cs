using System.ComponentModel.DataAnnotations;

namespace MVP_Core.Data.Models
{
    /// <summary>
    /// Represents a customer profile within the system.
    /// Includes basic contact details.
    /// </summary>
    public class Customer
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Customer name is required.")]
        [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        [MaxLength(20)]
        public string Phone { get; set; } = string.Empty;
    }
}
