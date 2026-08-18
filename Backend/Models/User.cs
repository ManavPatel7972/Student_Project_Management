using System;
using System.Collections.Generic;

namespace Backend.Models
{
    public class User
    {
        public int Id { get; set; }
        public int UserTypeId { get; set; }
        public int? DepartmentId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? UserCode { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;
        public string? ProfilePicturePath { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }


        public UserType UserType { get; set; } = null!;
        public Department? Department { get; set; }
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public ICollection<ProjectAllocation> StudentAllocations { get; set; } = new List<ProjectAllocation>();
        public ICollection<ProjectAllocation> FacultyAllocations { get; set; } = new List<ProjectAllocation>();
    }
}
