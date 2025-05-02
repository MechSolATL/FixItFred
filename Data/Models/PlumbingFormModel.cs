using System.ComponentModel.DataAnnotations;

namespace MVP_Core.Data.Models.ViewModels
{
    public class PlumbingFormModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [MaxLength(150, ErrorMessage = "Email cannot exceed 150 characters.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number.")]
        [MaxLength(20, ErrorMessage = "Phone number cannot exceed 20 characters.")]
        public string Phone { get; set; } = string.Empty;

        [MaxLength(10, ErrorMessage = "Verification code cannot exceed 10 characters.")]
        public string? VerificationCode { get; set; }
    }
}
