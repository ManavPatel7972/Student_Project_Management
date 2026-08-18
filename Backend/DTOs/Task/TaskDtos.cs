using System;

namespace Backend.DTOs.Task
{
    public class CreateTaskDto
    {
        public int ProjectAllocationId { get; set; }
        public string TaskTitle { get; set; } = string.Empty;
        public string? TaskDescription { get; set; }
        public int TaskStatusId { get; set; }
        public int TaskPriorityId { get; set; }
        public decimal AssignedScore { get; set; } = 0;
        public DateTime? TaskStartDate { get; set; }
        public DateTime? TaskDueDate { get; set; }
        public string? FacultyRemarks { get; set; }
    }

    public class UpdateTaskDto
    {
        public string TaskTitle { get; set; } = string.Empty;
        public string? TaskDescription { get; set; }
        public int TaskStatusId { get; set; }
        public int TaskPriorityId { get; set; }
        public decimal AssignedScore { get; set; }
        public decimal? EarnedScore { get; set; }
        public DateTime? TaskStartDate { get; set; }
        public DateTime? TaskDueDate { get; set; }
        public DateTime? TaskCompletedDate { get; set; }
        public DateTime? NextFollowUpDate { get; set; }
        public string? FacultyRemarks { get; set; }
        public string? StudentRemarks { get; set; }
    }

    public class UpdateTaskStatusDto
    {
        public int TaskStatusId { get; set; }
    }

    public class UpdateEarnedScoreDto
    {
        public decimal EarnedScore { get; set; }
        public string? FacultyRemarks { get; set; }
    }

    public class UpdateStudentRemarksDto
    {
        public string StudentRemarks { get; set; } = string.Empty;
    }

    public class TaskResponseDto
    {
        public int Id { get; set; }
        public int ProjectAllocationId { get; set; }
        public int? ProjectId { get; set; } = null;
        public string ProjectTitle { get; set; } = string.Empty;

        public int? StudentId { get; set; } = null;
        public string StudentName { get; set; } = string.Empty;

        public int? FacultyId { get; set; } = null;
        public string FacultyName { get; set; } = string.Empty;

        public string TaskTitle { get; set; } = string.Empty;
        public string? TaskDescription { get; set; }

        public int TaskStatusId { get; set; }
        public string TaskStatusName { get; set; } = string.Empty;
        public string TaskStatusCssClass { get; set; } = string.Empty;

        public int TaskPriorityId { get; set; }
        public string TaskPriorityName { get; set; } = string.Empty;
        public string TaskPriorityCssClass { get; set; } = string.Empty;

        public decimal AssignedScore { get; set; }
        public decimal? EarnedScore { get; set; }

        public DateTime TaskAssignedDate { get; set; }
        public DateTime? TaskStartDate { get; set; }
        public DateTime? TaskDueDate { get; set; }
        public DateTime? TaskCompletedDate { get; set; }
        public DateTime? NextFollowUpDate { get; set; }

        public string? FacultyRemarks { get; set; }
        public string? StudentRemarks { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
