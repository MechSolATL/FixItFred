namespace Data.Models
{
    public class RoleViewModel
    {
        public string RoleName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsSystemRole { get; set; } = false;
    }
}
