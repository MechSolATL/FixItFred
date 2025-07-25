namespace MVP_Core.Models.Admin
{
    public class DispatcherBroadcast
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Message is required.")]
        [StringLength(500, ErrorMessage = "Message must be under 500 characters.")]
        public required string Message { get; set; } = string.Empty;
        public DateTime IssuedAt { get; set; }
        [Display(Name = "Issued By")]
        public required string IssuedBy { get; set; } = string.Empty;
        [Display(Name = "Expires At")]
        public DateTime? ExpiresAt { get; set; }
        public bool IsActive => !ExpiresAt.HasValue || ExpiresAt > DateTime.UtcNow;
    }
}
