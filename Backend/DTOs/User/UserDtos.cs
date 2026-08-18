using System;

namespace Backend.DTOs.User
{
    public class CreateUserDto
    {
        public int UserTypeId { get; set; }
        public int? DepartmentId { get; set; }
        public int RoleId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? UserCode { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }

    public class UpdateUserDto
    {
        public int UserTypeId { get; set; }
        public int? DepartmentId { get; set; }
        public int RoleId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? UserCode { get; set; }
        public string Email { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }

    public class UserResponseDto
    {
        public int Id { get; set; }
        public int UserTypeId { get; set; }
        public string UserTypeName { get; set; } = string.Empty;
        public int? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? UserCode { get; set; }
        public string Email { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;
        public string? ProfilePicturePath { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class UpdateUserpatchDto
    {
        public int? UserTypeId { get; set; }
        public int? DepartmentId { get; set; }
        public int? RoleId { get; set; }
        public string? FullName { get; set; }
        public string? UserCode { get; set; }
        public string? Email { get; set; }
        public string? MobileNumber { get; set; }   
        public bool? isActive { get; set; }   

    }
}
