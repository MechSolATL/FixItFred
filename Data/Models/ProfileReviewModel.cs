namespace Data.Models
{
    /// <summary>
    /// Represents the profile information collected during the profile review process.
    /// Used to validate and temporarily store user inputs before submission.
    /// </summary>
    public class ProfileReviewModel
    {
        /// <summary>
        /// Gets or sets the user's chosen username.
        /// Must be between 3 and 20 characters.
        /// </summary>
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 20 characters.")]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the user's email address.
        /// Used for verification and contact purposes.
        /// </summary>
        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        [MaxLength(255, ErrorMessage = "Email address cannot exceed 255 characters.")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the user's phone number.
        /// Used for secondary verification.
        /// </summary>
        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        [MaxLength(20, ErrorMessage = "Phone number cannot exceed 20 characters.")]
        public string PhoneNumber { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the verification code sent to the user.
        /// Required to complete the verification process.
        /// </summary>
        [Required(ErrorMessage = "Verification code is required.")]
        [StringLength(10, ErrorMessage = "Verification code must not exceed 10 characters.")]
        public string VerificationCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets optional notes or comments entered by the user during the review.
        /// </summary>
        [StringLength(500, ErrorMessage = "Review notes should not exceed 500 characters.")]
        public string? ReviewNotes { get; set; }
    }
}
