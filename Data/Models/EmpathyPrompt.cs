using System.ComponentModel.DataAnnotations;

namespace Data.Models;

/// <summary>
/// Empathy prompt model for Lyra cognition testing
/// </summary>
public class EmpathyPrompt
{
    public Guid Id { get; set; }
    
    [Required]
    [StringLength(500)]
    public string Text { get; set; } = null!;
    
    [StringLength(100)]
    public string Category { get; set; } = null!;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public bool IsActive { get; set; } = true;
}