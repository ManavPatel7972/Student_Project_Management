using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Data;
using Backend.DTOs.Dashboard;
using Backend.DTOs.ProjectAllocation;
using Backend.DTOs.Task;
using Backend.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DashboardController : BasicInfoController
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        #region (Helper)GetRecentAllocationsQuery
        private IQueryable<AllocationResponseDto> GetRecentAllocationsQuery()
        {
            return _context.ProjectAllocations
                .Select(pa => new AllocationResponseDto
                {
                    Id = pa.Id,
                    ProjectId = pa.ProjectId,

                    ProjectTitle = pa.Project != null ? pa.Project.ProjectTitle : string.Empty,

                    ProjectStatus = pa.Project != null ? pa.Project.Status.ToString() : string.Empty,


                    StudentId = pa.StudentId,

                    StudentName = pa.Student != null ? pa.Student.FullName : string.Empty,

                    StudentCode = pa.Student != null ? pa.Student.UserCode : null,

                    StudentDepartment = pa.Student != null && pa.Student.Department != null ? pa.Student.Department.Name : string.Empty,


                    FacultyId = pa.FacultyId,

                    FacultyName = pa.Faculty != null ? pa.Faculty.FullName : string.Empty,

                    AssignedDate = pa.AssignedDate,

                    ProjectStartDate = pa.ProjectStartDate,

                    ProjectEndDate = pa.ProjectEndDate,

                    OverAllGrade = pa.OverAllGrade,

                    TotalTasksGiven = pa.Tasks.Count,


                    TotalCompletedTasks = pa.Tasks.Count(t => t.TaskStatus != null && t.TaskStatus.TaskStatusName == "Completed"),



                    ProgressPercentage = pa.Tasks.Count == 0 ? 0 : (decimal)pa.Tasks
                    .Count(t => t.TaskStatus != null && t.TaskStatus.TaskStatusName == "Completed") / pa.Tasks.Count * 100,


                    CreatedAt = pa.CreatedAt,


                }).OrderByDescending(pa => pa.CreatedAt).AsQueryable();
        }

        #endregion


        #region (Helper)GetRecentTasksQuery
        private IQueryable<TaskResponseDto> GetRecentTasksQuery()
        {
            return _context.Tasks
                .Select(t => new TaskResponseDto
                {
                    Id = t.Id,

                    ProjectAllocationId = t.ProjectAllocationId,

                    ProjectId = t.ProjectAllocation != null ? t.ProjectAllocation.ProjectId : null,

                    ProjectTitle = t.ProjectAllocation != null && t.ProjectAllocation.Project != null ? t.ProjectAllocation.Project.ProjectTitle : string.Empty,

                    StudentId = t.ProjectAllocation != null ? t.ProjectAllocation.StudentId : null,

                    StudentName = t.ProjectAllocation != null && t.ProjectAllocation.Student != null ? t.ProjectAllocation.Student.FullName : string.Empty,

                    FacultyId = t.ProjectAllocation != null ? t.ProjectAllocation.FacultyId : null,

                    FacultyName = t.ProjectAllocation != null && t.ProjectAllocation.Faculty != null ? t.ProjectAllocation.Faculty.FullName : string.Empty,

                    TaskTitle = t.TaskTitle,

                    TaskDescription = t.TaskDescription,

                    TaskStatusId = t.TaskStatusId,

                    TaskStatusName = t.TaskStatus != null ? t.TaskStatus.TaskStatusName : string.Empty,

                    TaskStatusCssClass = t.TaskStatus != null ? t.TaskStatus.TaskStatusCssClass : string.Empty,

                    TaskPriorityId = t.TaskPriorityId,

                    TaskPriorityName = t.TaskPriority != null ? t.TaskPriority.TaskPriorityName : string.Empty,

                    TaskPriorityCssClass = t.TaskPriority != null ? t.TaskPriority.TaskPriorityCssClass : string.Empty,

                    AssignedScore = t.AssignedScore,

                    EarnedScore = t.EarnedScore,

                    TaskAssignedDate = t.TaskAssignedDate,

                    TaskStartDate = t.TaskStartDate,

                    TaskDueDate = t.TaskDueDate,

                    TaskCompletedDate = t.TaskCompletedDate,

                    NextFollowUpDate = t.NextFollowUpDate,

                    FacultyRemarks = t.FacultyRemarks,

                    StudentRemarks = t.StudentRemarks,

                    CreatedAt = t.CreatedAt,


                }).OrderByDescending(t => t.CreatedAt).AsQueryable();
        }

        #endregion


        #region AdminDashboard
        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAdminDashboard()
        {
            try
            {
                var overview = new DashboardOverviewDto
                {
                    TotalUsers = await _context.Users
                    .CountAsync(),

                    TotalStudents = await _context.Users
                    .CountAsync(u => u.UserType.UserTypeName == "Student"),

                    TotalFaculty = await _context.Users
                    .CountAsync(u => u.UserType.UserTypeName == "Faculty"),

                    TotalProjects = await _context.Projects
                    .CountAsync(),

                    TotalAllocations = await _context.ProjectAllocations
                    .CountAsync(),

                    TotalTasks = await _context.Tasks
                    .CountAsync(),

                    ProjectsNotStarted = await _context.Projects
                    .CountAsync(p => p.Status == ProjectStatus.NotStarted),

                    ProjectsCompleted = await _context.Projects
                    .CountAsync(p => p.Status == ProjectStatus.Completed),

                    ProjectsInProgress = await _context.Projects
                    .CountAsync(p => p.Status == ProjectStatus.InProgress),

                    TasksLow = await _context.Tasks
                    .CountAsync(t => t.TaskPriority.TaskPriorityName == "Low"),

                    TasksMedium = await _context.Tasks
                    .CountAsync(t => t.TaskPriority.TaskPriorityName == "Medium"),

                    TasksHigh = await _context.Tasks
                    .CountAsync(t => t.TaskPriority.TaskPriorityName == "High"),

                    TasksCritical = await _context.Tasks
                    .CountAsync(t => t.TaskPriority.TaskPriorityName == "Critical"),

                    TasksPending = await _context.Tasks
                    .CountAsync(t => t.TaskStatus.TaskStatusName == "Pending"),

                    TasksInProgress = await _context.Tasks
                    .CountAsync(t => t.TaskStatus.TaskStatusName == "In Progress"),

                    TasksCompleted = await _context.Tasks
                    .CountAsync(t => t.TaskStatus.TaskStatusName == "Completed"),

                };

                var recentAllocations = await GetRecentAllocationsQuery()
                    .Take(5)
                    .ToListAsync();

                var recentTasks = await GetRecentTasksQuery()
                    .Take(5)
                    .ToListAsync();


                var result = new AdminDashboardDto
                {
                    Overview = overview,
                    RecentAllocations = recentAllocations,
                    RecentTasks = recentTasks
                };

                return Ok(new ApiResponse<AdminDashboardDto>
                {
                    Success = true,
                    StatusCode = 200,
                    Data = result,
                    Message = "Admin Dashboard Fetched Successfully."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
                {
                    Success = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "An error occurred while processing your request.",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        #endregion


        #region StudentDashboard

        [HttpGet("student")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetStudentDashboard()
        {
            try
            {
                var currentUserId = GetCurrentUserId();

                if (!currentUserId.HasValue)
                {
                    return Unauthorized(new ApiResponse<object>
                    {
                        Success = false,
                        StatusCode = 401,
                        Errors = new List<string>
                        {
                            "Unauthorized."
                        }
                    });
                }

                var studentAllocations = _context.ProjectAllocations
                    .Where(pa => pa.StudentId == currentUserId.Value);

                var studentTasks = _context.Tasks
                    .Where(t => t.ProjectAllocation.StudentId == currentUserId.Value);

                var overview = new DashboardOverviewDto
                {
                    TotalProjects = await studentAllocations
                    .Select(pa => pa.ProjectId)
                    .Distinct()
                    .CountAsync(),

                    TotalAllocations = await studentAllocations
                    .CountAsync(),


                    TotalTasks = await studentTasks
                    .CountAsync(),

                    ProjectsNotStarted = await studentAllocations
                    .CountAsync(pa => pa.Project.Status == ProjectStatus.NotStarted),

                    ProjectsInProgress = await studentAllocations
                    .CountAsync(pa => pa.Project.Status == ProjectStatus.InProgress),

                    ProjectsCompleted = await studentAllocations
                    .CountAsync(pa => pa.Project.Status == ProjectStatus.Completed),

                    TasksLow = await studentTasks
                    .CountAsync(t => t.TaskPriority.TaskPriorityName == "Low"),

                    TasksMedium = await studentTasks
                    .CountAsync(t => t.TaskPriority.TaskPriorityName == "Medium"),

                    TasksHigh = await studentTasks
                    .CountAsync(t => t.TaskPriority.TaskPriorityName == "High"),

                    TasksCritical = await studentTasks
                    .CountAsync(t => t.TaskPriority.TaskPriorityName == "Critical"),

                    TasksPending = await studentTasks
                    .CountAsync(t => t.TaskStatus.TaskStatusName == "Pending"),

                    TasksInProgress = await studentTasks
                    .CountAsync(t => t.TaskStatus.TaskStatusName == "In Progress"),

                    TasksCompleted = await studentTasks
                    .CountAsync(t => t.TaskStatus.TaskStatusName == "Completed"),
                };


                var recentAllocations = await GetRecentAllocationsQuery()
                   .Where(pa => pa.StudentId == currentUserId.Value)
                   .Take(5)
                   .ToListAsync();


                var recentTasks = await GetRecentTasksQuery()
                    .Where(t => t.StudentId == currentUserId.Value)
                    .Take(5)
                    .ToListAsync();


                var result = new StudentDashboardDto
                {
                    Overview = overview,
                    RecentAllocations = recentAllocations,
                    RecentTasks = recentTasks
                };


                return Ok(new ApiResponse<StudentDashboardDto>
                {
                    Success = true,
                    StatusCode = 200,
                    Data = result,
                    Message = "Student Dashboard Fetched Successfully."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
                {
                    Success = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "An error occurred while processing your request.",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
        #endregion


        #region FacultyDashboard
        [HttpGet("faculty")]
        [Authorize(Roles = "Faculty")]
        public async Task<IActionResult> GetFacultyDashboard()
        {
            try
            {
                var currentUserId = GetCurrentUserId();

                if (!currentUserId.HasValue)
                {
                    return Unauthorized(new ApiResponse<object>
                    {
                        Success = false,
                        StatusCode = 401,
                        Errors = new List<string>
                        {
                            "Unauthorized."
                        }
                    });
                }


                var facultyAllocations = _context.ProjectAllocations
                    .Where(pa => pa.FacultyId == currentUserId.Value);


                var facultyTasks = _context.Tasks
                    .Where(t => t.ProjectAllocation.FacultyId == currentUserId.Value);


                var overview = new DashboardOverviewDto
                {
                    TotalProjects = await facultyAllocations
                    .Select(pa => pa.ProjectId)
                    .Distinct()
                    .CountAsync(),

                    TotalAllocations = await facultyAllocations
                    .CountAsync(),

                    TotalTasks = await facultyTasks.
                    CountAsync(),


                    ProjectsNotStarted = await facultyAllocations
                    .CountAsync(pa => pa.Project.Status == ProjectStatus.NotStarted),

                    ProjectsInProgress = await facultyAllocations
                    .CountAsync(pa => pa.Project.Status == ProjectStatus.InProgress),

                    ProjectsCompleted = await facultyAllocations
                    .CountAsync(pa => pa.Project.Status == ProjectStatus.Completed),

                    TasksLow = await facultyTasks
                    .CountAsync(t => t.TaskPriority.TaskPriorityName == "Low"),

                    TasksMedium = await facultyTasks
                    .CountAsync(t => t.TaskPriority.TaskPriorityName == "Medium"),


                    TasksHigh = await facultyTasks
                    .CountAsync(t => t.TaskPriority.TaskPriorityName == "High"),

                    TasksCritical = await facultyTasks
                    .CountAsync(t => t.TaskPriority.TaskPriorityName == "Critical"),

                    TasksPending = await facultyTasks
                    .CountAsync(t => t.TaskStatus.TaskStatusName == "Pending"),

                    TasksInProgress = await facultyTasks
                    .CountAsync(t => t.TaskStatus.TaskStatusName == "In Progress"),

                    TasksCompleted = await facultyTasks
                    .CountAsync(t => t.TaskStatus.TaskStatusName == "Completed")

                };


                var recentAllocations =
                    await GetRecentAllocationsQuery()
                    .Where(pa => pa.FacultyId == currentUserId.Value)
                    .Take(5)
                    .ToListAsync();


                var recentTasks =
                    await GetRecentTasksQuery()
                    .Where(t => t.FacultyId == currentUserId.Value)
                    .Take(5)
                    .ToListAsync();


                var result = new FacultyDashboardDto
                {
                    Overview = overview,
                    RecentAllocations = recentAllocations,
                    RecentTasks = recentTasks
                };


                return Ok(new ApiResponse<FacultyDashboardDto>
                {
                    Success = true,
                    StatusCode = 200,
                    Data = result,
                    Message = "Faculty Dashboard Fetched Successfully."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
                {
                    Success = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "An error occurred while processing your request.",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        #endregion

    }

}
