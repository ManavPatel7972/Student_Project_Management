using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Backend.Data;
using Backend.DTOs.Task;
using Backend.Helpers;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TasksController : BasicInfoController
    {
        private readonly ApplicationDbContext _context;

        public TasksController(ApplicationDbContext context)
        {
            _context = context;
        }

        private static TaskResponseDto MapToDto(TaskItem t)
        {
            return new TaskResponseDto
            {
                Id = t.Id,
                ProjectAllocationId = t.ProjectAllocationId,
                ProjectId = t.ProjectAllocation != null ? t.ProjectAllocation.ProjectId : 0,
                ProjectTitle = t.ProjectAllocation != null && t.ProjectAllocation.Project != null ? t.ProjectAllocation.Project.ProjectTitle : string.Empty,
                StudentId = t.ProjectAllocation != null ? t.ProjectAllocation.StudentId : 0,
                StudentName = t.ProjectAllocation != null && t.ProjectAllocation.Student != null ? t.ProjectAllocation.Student.FullName : string.Empty,
                FacultyId = t.ProjectAllocation != null ? t.ProjectAllocation.FacultyId : 0,
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
                CreatedAt = t.CreatedAt
            };
        }

        private Task<IActionResult?> EnsureTaskAccessAsync(TaskItem task)
        {
            if (!IsStudent()) return Task.FromResult<IActionResult?>(null);

            var currentUserId = GetCurrentUserId();
            if (!currentUserId.HasValue ||
                task.ProjectAllocation == null ||
                task.ProjectAllocation.StudentId != currentUserId.Value)
            {
                return Task.FromResult<IActionResult?>(new ObjectResult(new ApiResponse<object>
                {
                    Success = false,
                    StatusCode = 403,
                    Errors = new List<string> { "Access denied." }
                })
                { StatusCode = 403 });
            }

            return Task.FromResult<IActionResult?>(null);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? search,
            [FromQuery] int? allocationId,
            [FromQuery] int? studentId,
            [FromQuery] int? facultyId,
            [FromQuery] int? statusId,
            [FromQuery] int? priorityId)
        {
            var query = _context.Tasks
                .Include(t => t.ProjectAllocation)
                    .ThenInclude(pa => pa.Project)
                .Include(t => t.ProjectAllocation)
                    .ThenInclude(pa => pa.Student)
                .Include(t => t.ProjectAllocation)
                    .ThenInclude(pa => pa.Faculty)
                .Include(t => t.TaskStatus)
                .Include(t => t.TaskPriority)
                .AsQueryable();

            if (IsStudent())
            {
                var currentUserId = GetCurrentUserId();
                if (!currentUserId.HasValue) return Unauthorized(new ApiResponse<object> { Success = false, StatusCode = 401, Errors = new List<string> { "Unauthorized." } });
                query = query.Where(t => t.ProjectAllocation.StudentId == currentUserId.Value);
            }
            else
            {
                if (allocationId.HasValue) query = query.Where(t => t.ProjectAllocationId == allocationId.Value);
                if (studentId.HasValue) query = query.Where(t => t.ProjectAllocation.StudentId == studentId.Value);
                if (facultyId.HasValue) query = query.Where(t => t.ProjectAllocation.FacultyId == facultyId.Value);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                query = query.Where(t => t.TaskTitle.ToLower().Contains(s) || (t.TaskDescription != null && t.TaskDescription.ToLower().Contains(s)));
            }

            if (statusId.HasValue) query = query.Where(t => t.TaskStatusId == statusId.Value);
            if (priorityId.HasValue) query = query.Where(t => t.TaskPriorityId == priorityId.Value);

            var tasks = await query.OrderByDescending(t => t.CreatedAt).ToListAsync();
            return Ok(new ApiResponse<List<TaskResponseDto>> { Success = true, Data = tasks.Select(MapToDto).ToList() });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var task = await _context.Tasks
                .Include(t => t.ProjectAllocation)
                    .ThenInclude(pa => pa.Project)
                .Include(t => t.ProjectAllocation)
                    .ThenInclude(pa => pa.Student)
                .Include(t => t.ProjectAllocation)
                    .ThenInclude(pa => pa.Faculty)
                .Include(t => t.TaskStatus)
                .Include(t => t.TaskPriority)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null) return NotFound(new ApiResponse<object> { Success = false, StatusCode = 404, Errors = new List<string> { "Task not found." } });

            var accessDenied = await EnsureTaskAccessAsync(task);
            if (accessDenied != null) return accessDenied;

            return Ok(new ApiResponse<TaskResponseDto> { Success = true, Data = MapToDto(task) });
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Faculty")]
        public async Task<IActionResult> Create([FromBody] CreateTaskDto dto)
        {
            var task = new TaskItem
            {
                ProjectAllocationId = dto.ProjectAllocationId,
                TaskTitle = dto.TaskTitle,
                TaskDescription = dto.TaskDescription,
                TaskStatusId = dto.TaskStatusId,
                TaskPriorityId = dto.TaskPriorityId,
                AssignedScore = dto.AssignedScore,
                TaskStartDate = dto.TaskStartDate,
                TaskDueDate = dto.TaskDueDate,
                FacultyRemarks = dto.FacultyRemarks,
                TaskAssignedDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            var created = await _context.Tasks
                .Include(t => t.ProjectAllocation)
                    .ThenInclude(pa => pa.Project)
                .Include(t => t.ProjectAllocation)
                    .ThenInclude(pa => pa.Student)
                .Include(t => t.ProjectAllocation)
                    .ThenInclude(pa => pa.Faculty)
                .Include(t => t.TaskStatus)
                .Include(t => t.TaskPriority)
                .FirstAsync(t => t.Id == task.Id);

            return CreatedAtAction(nameof(GetById), new { id = task.Id }, new ApiResponse<TaskResponseDto>
            {
                Success = true,
                StatusCode = 201,
                Data = MapToDto(created),
                Message = "Task created successfully."
            });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Faculty")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTaskDto dto)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null) return NotFound(new ApiResponse<object> { Success = false, StatusCode = 404, Errors = new List<string> { "Task not found." } });

            task.TaskTitle = dto.TaskTitle;
            task.TaskDescription = dto.TaskDescription;
            task.TaskStatusId = dto.TaskStatusId;
            task.TaskPriorityId = dto.TaskPriorityId;
            task.AssignedScore = dto.AssignedScore;
            task.EarnedScore = dto.EarnedScore;
            task.TaskStartDate = dto.TaskStartDate;
            task.TaskDueDate = dto.TaskDueDate;
            task.TaskCompletedDate = dto.TaskCompletedDate;
            task.NextFollowUpDate = dto.NextFollowUpDate;
            task.FacultyRemarks = dto.FacultyRemarks;
            task.StudentRemarks = dto.StudentRemarks;
            task.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var updated = await _context.Tasks
                .Include(t => t.ProjectAllocation)
                    .ThenInclude(pa => pa.Project)
                .Include(t => t.ProjectAllocation)
                    .ThenInclude(pa => pa.Student)
                .Include(t => t.ProjectAllocation)
                    .ThenInclude(pa => pa.Faculty)
                .Include(t => t.TaskStatus)
                .Include(t => t.TaskPriority)
                .FirstAsync(t => t.Id == task.Id);

            return Ok(new ApiResponse<TaskResponseDto> { Success = true, Data = MapToDto(updated), Message = "Task updated successfully." });
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateTaskStatusDto dto)
        {
            var task = await _context.Tasks
                .Include(t => t.TaskStatus)
                .Include(t => t.ProjectAllocation)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null) return NotFound(new ApiResponse<object> { Success = false, StatusCode = 404, Errors = new List<string> { "Task not found." } });

            var accessDenied = await EnsureTaskAccessAsync(task);
            if (accessDenied != null) return accessDenied;

            task.TaskStatusId = dto.TaskStatusId;
            task.UpdatedAt = DateTime.UtcNow;

            // If completed, set completed date
            var statusLookup = await _context.TaskStatuses.FindAsync(dto.TaskStatusId);
            if (statusLookup != null && statusLookup.TaskStatusName == "Completed")
            {
                task.TaskCompletedDate = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return Ok(new ApiResponse<object> { Success = true, Message = "Task status updated successfully." });
        }

        [HttpPatch("{id}/earned-score")]
        [Authorize(Roles = "Admin,Faculty")]
        public async Task<IActionResult> UpdateEarnedScore(int id, [FromBody] UpdateEarnedScoreDto dto)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null) return NotFound(new ApiResponse<object> { Success = false, StatusCode = 404, Errors = new List<string> { "Task not found." } });

            task.EarnedScore = dto.EarnedScore;
            if (!string.IsNullOrEmpty(dto.FacultyRemarks))
            {
                task.FacultyRemarks = dto.FacultyRemarks;
            }
            task.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(new ApiResponse<object> { Success = true, Message = "Earned score updated successfully." });
        }

        [HttpPatch("{id}/student-remarks")]
        public async Task<IActionResult> UpdateStudentRemarks(int id, [FromBody] UpdateStudentRemarksDto dto)
        {
            var task = await _context.Tasks
                .Include(t => t.ProjectAllocation)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null) return NotFound(new ApiResponse<object> { Success = false, StatusCode = 404, Errors = new List<string> { "Task not found." } });

            var accessDenied = await EnsureTaskAccessAsync(task);
            if (accessDenied != null) return accessDenied;

            task.StudentRemarks = dto.StudentRemarks;
            task.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(new ApiResponse<object> { Success = true, Message = "Student remarks updated successfully." });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null) return NotFound(new ApiResponse<object> { Success = false, StatusCode = 404, Errors = new List<string> { "Task not found." } });

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            return Ok(new ApiResponse<object> { Success = true, Message = "Task deleted successfully." });
        }
    }
}
