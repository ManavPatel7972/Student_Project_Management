using System;
using System.Collections.Generic;
using Backend.Enums;

namespace Backend.Models
{
    public class ProjectMaster
    {
        public int Id { get; set; }
        public string ProjectTitle { get; set; } = string.Empty;
        public string? Description { get; set; }
        public ProjectStatus Status { get; set; } = ProjectStatus.NotStarted;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public ICollection<ProjectAllocation> Allocations { get; set; } = new List<ProjectAllocation>();
    }
}
