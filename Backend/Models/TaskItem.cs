using System;

namespace Backend.Models
{
    public class TaskItem
    {
        public int Id { get; set; }
        public int ProjectAllocationId { get; set; }
        public string TaskTitle { get; set; } = string.Empty;
        public string? TaskDescription { get; set; }

        public int TaskStatusId { get; set; }
        public int TaskPriorityId { get; set; }

        public decimal AssignedScore { get; set; } = 0;
        public decimal? EarnedScore { get; set; }

        public DateTime TaskAssignedDate { get; set; } = DateTime.UtcNow;
        public DateTime? TaskStartDate { get; set; }
        public DateTime? TaskDueDate { get; set; }
        public DateTime? TaskCompletedDate { get; set; }
        public DateTime? NextFollowUpDate { get; set; }

        public string? FacultyRemarks { get; set; }
        public string? StudentRemarks { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation Properties
        public ProjectAllocation ProjectAllocation { get; set; } = null!;
        public TaskStatusLookup TaskStatus { get; set; } = null!;
        public TaskPriorityLookup TaskPriority { get; set; } = null!;
    }
}
