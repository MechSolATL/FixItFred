using System.ComponentModel.DataAnnotations;

namespace MVP_Core.Data.Models
{
    public class PlumbingFormModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; } = string.Empty;
    }
}
