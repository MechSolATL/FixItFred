using System.ComponentModel.DataAnnotations;

namespace MVP_Core.Data.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Username is required.")]
        [MaxLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;
    }
}
