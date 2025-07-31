namespace Data.Models
{
    public class ProfileReview
    {
        /// <summary>
        /// The unique identifier for the profile review.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The username associated with the profile review.
        /// </summary>
        [Required(ErrorMessage = "Username is required.")]
        [MaxLength(100, ErrorMessage = "Username cannot exceed 100 characters.")]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// The email address associated with the profile review.
        /// </summary>
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        [MaxLength(255, ErrorMessage = "Email cannot exceed 255 characters.")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// The phone number associated with the profile review.
        /// </summary>
        [Phone(ErrorMessage = "Invalid phone number format.")]
        [MaxLength(20, ErrorMessage = "Phone number cannot exceed 20 characters.")]
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// The verification code for the profile review, if applicable.
        /// </summary>
        [MaxLength(10, ErrorMessage = "Verification code cannot exceed 10 characters.")]
        public string? VerificationCode { get; set; }

        /// <summary>
        /// Notes associated with the profile review.
        /// </summary>
        [MaxLength(500, ErrorMessage = "Review notes cannot exceed 500 characters.")]
        public string? ReviewNotes { get; set; }

        /// <summary>
        /// The timestamp when the profile review was submitted.
        /// </summary>
        [Required]
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// The timestamp when the profile review was created.
        /// </summary>
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
