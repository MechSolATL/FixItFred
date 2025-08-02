// [Sprint1002_FixItFred] Created missing UserContext model to resolve ReplayEngineService compilation error
// This model was referenced but missing from codebase
// Sprint1002: Add missing model definitions with null validation

using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class UserContext
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "User ID is required.")]
        public int UserId { get; set; }

        [MaxLength(100)]
        public string? UserName { get; set; }

        [MaxLength(50)]
        public string? Role { get; set; } = "User";

        [MaxLength(50)]
        public string? TenantId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;

        [MaxLength(200)]
        public string? AdditionalData { get; set; }
    }
}