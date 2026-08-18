using System;

namespace Backend.DTOs.ProjectAllocation
{
    public class CreateAllocationDto
    {
        public int ProjectId { get; set; }
        public int StudentId { get; set; }
        public int FacultyId { get; set; }
        public DateTime ProjectStartDate { get; set; }
        public DateTime ProjectEndDate { get; set; }
    }

    public class UpdateAllocationDto
    {
        public DateTime ProjectStartDate { get; set; }
        public DateTime ProjectEndDate { get; set; }
        public string? OverAllGrade { get; set; }
    }

    public class AllocationResponseDto
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string ProjectTitle { get; set; } = string.Empty;
        public string ProjectStatus { get; set; } = string.Empty;

        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string? StudentCode { get; set; }
        public string? StudentDepartment { get; set; }

        public int FacultyId { get; set; }
        public string FacultyName { get; set; } = string.Empty;

        public DateTime AssignedDate { get; set; }
        public DateTime ProjectStartDate { get; set; }
        public DateTime ProjectEndDate { get; set; }
        public string? OverAllGrade { get; set; }

        public int TotalTasksGiven { get; set; }
        public int TotalCompletedTasks { get; set; }
        public decimal ProgressPercentage { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
