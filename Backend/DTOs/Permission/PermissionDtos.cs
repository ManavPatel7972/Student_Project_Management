using System.Collections.Generic;
using Backend.DTOs.Role;

namespace Backend.DTOs.Permission
{
    public class PermissionResponseDto
    {
        public int Id { get; set; }
        public string PermissionName { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    public class UpdateRolePermissionDto
    {
        public int RoleId { get; set; }
        public int PermissionId { get; set; }
        public bool IsGranted { get; set; }
    }

    public class RolePermissionMatrixDto
    {
        public List<RoleResponseDto> Roles { get; set; } = new();
        public List<PermissionResponseDto> Permissions { get; set; } = new();
        public List<RolePermissionItemDto> RolePermissions { get; set; } = new();
    }

    public class RolePermissionItemDto
    {
        public int RoleId { get; set; }
        public int PermissionId { get; set; }
        public bool IsGranted { get; set; }
    }
}
