using System.Collections.Generic;
using Backend.DTOs.ProjectAllocation;
using Backend.DTOs.Task;

namespace Backend.DTOs.Dashboard
{
    public class DashboardOverviewDto
    {
        // Users
        public int TotalUsers { get; set; }
        public int TotalStudents { get; set; }
        public int TotalFaculty { get; set; }

        // Projects
        public int TotalProjects { get; set; }
        public int TotalAllocations { get; set; }

        // Tasks
        public int TotalTasks { get; set; }

        // Project status
        public int ProjectsNotStarted { get; set; }
        public int ProjectsInProgress { get; set; }
        public int ProjectsCompleted { get; set; }

        // Task priority
        public int TasksLow { get; set; }
        public int TasksMedium { get; set; }
        public int TasksHigh { get; set; }
        public int TasksCritical { get; set; }

        // Task status
        public int TasksPending { get; set; }
        public int TasksInProgress { get; set; }
        public int TasksCompleted { get; set; }
    }


    public class StudentDashboardDto
    {
        public DashboardOverviewDto Overview { get; set; } = new DashboardOverviewDto();

        public List<AllocationResponseDto> RecentAllocations { get; set; } = new List<AllocationResponseDto>();

        public List<TaskResponseDto> RecentTasks { get; set; } = new List<TaskResponseDto>();
    }

    public class FacultyDashboardDto
    {
        public DashboardOverviewDto Overview { get; set; } = new DashboardOverviewDto();

        public List<AllocationResponseDto> RecentAllocations { get; set; } = new List<AllocationResponseDto>();

        public List<TaskResponseDto> RecentTasks { get; set; } = new List<TaskResponseDto>();
    }

    public class AdminDashboardDto
    {
        public DashboardOverviewDto Overview { get; set; } = new DashboardOverviewDto();

        public List<AllocationResponseDto> RecentAllocations { get; set; } = new List<AllocationResponseDto>();

        public List<TaskResponseDto> RecentTasks { get; set; } = new List<TaskResponseDto>();
    }


}
