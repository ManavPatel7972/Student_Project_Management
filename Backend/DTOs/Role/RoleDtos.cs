namespace Backend.DTOs.Role
{
    public class CreateRoleDto
    {
        public string RoleName { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    public class UpdateRoleDto
    {
        public string RoleName { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    public class RoleResponseDto
    {
        public int Id { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int UserCount { get; set; }
    }
}
