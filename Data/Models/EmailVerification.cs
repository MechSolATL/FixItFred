namespace MVP_Core.Data.Models
{
    /// <summary>
    /// Stores email verification codes and status for users.
    /// </summary>
    [Index(nameof(Email))]
    public class EmailVerification
    {
        public int Id { get; set; }

        /// <summary>
        /// The email address being verified.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// The unique verification code sent to the user.
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Indicates whether the email has been successfully verified.
        /// </summary>
        public bool IsVerified { get; set; } = false;

        /// <summary>
        /// Timestamp when the verification record was created.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Expiration time for the verification code.
        /// </summary>
        public DateTime Expiration { get; set; } = DateTime.UtcNow.AddMinutes(15); // default 15-minute validity
    }
}
