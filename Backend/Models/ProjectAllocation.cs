using System;
using System.Collections.Generic;

namespace Backend.Models
{
    public class ProjectAllocation
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int StudentId { get; set; }
        public int FacultyId { get; set; }

        public DateTime AssignedDate { get; set; } = DateTime.UtcNow;
        public DateTime ProjectStartDate { get; set; }
        public DateTime ProjectEndDate { get; set; }

        public string? OverAllGrade { get; set; } // 'A', 'B', 'C', etc.
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation Properties
        public ProjectMaster Project { get; set; } = null!;
        public User Student { get; set; } = null!;
        public User Faculty { get; set; } = null!;

        public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    }
}
