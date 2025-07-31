using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models
{
    [Table("ProfileReviews")]
    public class ProfileReviewEntity
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        [StringLength(100, ErrorMessage = "Username cannot exceed 100 characters.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [StringLength(255, ErrorMessage = "Email cannot exceed 255 characters.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters.")]
        public string PhoneNumber { get; set; } = string.Empty;

        [StringLength(10, ErrorMessage = "Verification code cannot exceed 10 characters.")]
        public string VerificationCode { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Review notes cannot exceed 500 characters.")]
        public string? ReviewNotes { get; set; }

        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    }
}
