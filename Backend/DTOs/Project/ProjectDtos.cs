using System;
using Backend.Enums;

namespace Backend.DTOs.Project
{
    public class CreateProjectDto
    {
        public string ProjectTitle { get; set; } = string.Empty;
        public string? Description { get; set; }
        public ProjectStatus Status { get; set; } = ProjectStatus.NotStarted;
    }

    public class UpdateProjectDto
    {
        public string ProjectTitle { get; set; } = string.Empty;
        public string? Description { get; set; }
        public ProjectStatus Status { get; set; }
    }

    public class UpdateProjectStatusDto
    {
        public ProjectStatus Status { get; set; }
    }

    public class ProjectResponseDto
    {
        public int Id { get; set; }
        public string ProjectTitle { get; set; } = string.Empty;
        public string? Description { get; set; }
        public ProjectStatus Status { get; set; }
        public string StatusName => Status.ToString();
        public int TotalAllocations { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
