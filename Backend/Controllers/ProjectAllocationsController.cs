using Backend.Data;
using Backend.DTOs.ProjectAllocation;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProjectAllocationsController : BasicInfoController
    {
        private readonly ApplicationDbContext _context;

        public ProjectAllocationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        private static AllocationResponseDto MapToDto(ProjectAllocation pa)
        {
            var totalTasks = pa.Tasks != null ? pa.Tasks.Count : 0;
            var completedTasks = pa.Tasks != null ? pa.Tasks.Count(t => t.TaskStatus != null && t.TaskStatus.TaskStatusName == "Completed") : 0;
            var progress = totalTasks == 0 ? 0 : (decimal)completedTasks * 100 / totalTasks;

            return new AllocationResponseDto
            {
                Id = pa.Id,
                ProjectId = pa.ProjectId,
                ProjectTitle = pa.Project != null ? pa.Project.ProjectTitle : string.Empty,
                ProjectStatus = pa.Project != null ? pa.Project.Status.ToString() : string.Empty,
                StudentId = pa.StudentId,
                StudentName = pa.Student != null ? pa.Student.FullName : string.Empty,
                StudentCode = pa.Student != null ? pa.Student.UserCode : null,
                StudentDepartment = pa.Student != null && pa.Student.Department != null ? pa.Student.Department.Name : null,
                FacultyId = pa.FacultyId,
                FacultyName = pa.Faculty != null ? pa.Faculty.FullName : string.Empty,
                AssignedDate = pa.AssignedDate,
                ProjectStartDate = pa.ProjectStartDate,
                ProjectEndDate = pa.ProjectEndDate,
                OverAllGrade = pa.OverAllGrade,
                TotalTasksGiven = totalTasks,
                TotalCompletedTasks = completedTasks,
                ProgressPercentage = progress,
                CreatedAt = pa.CreatedAt
            };
        }

        #region GetAllAllocstions
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? projectId, [FromQuery] int? studentId, [FromQuery] int? facultyId)
        {
            var query = _context.ProjectAllocations
                .Include(pa => pa.Project)
                .Include(pa => pa.Student)
                .ThenInclude(s => s.Department)
                .Include(pa => pa.Faculty)
                .Include(pa => pa.Tasks)
                .ThenInclude(t => t.TaskStatus)
                .AsQueryable();

            if (IsStudent())
            {
                var currentUserId = GetCurrentUserId();
                if (!currentUserId.HasValue) return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    StatusCode = 401,
                    Errors = new List<string> { "Unauthorized." }
                });

                query = query.Where(pa => pa.StudentId == currentUserId.Value);
            }
            else
            {
                if (projectId.HasValue) query = query.Where(pa => pa.ProjectId == projectId.Value);
                if (studentId.HasValue) query = query.Where(pa => pa.StudentId == studentId.Value);
                if (facultyId.HasValue) query = query.Where(pa => pa.FacultyId == facultyId.Value);
            }

            var allocations = await query.OrderByDescending(pa => pa.CreatedAt).ToListAsync();

            return Ok(new ApiResponse<List<AllocationResponseDto>>
            {
                Success = true,
                Data = allocations.Select(MapToDto).ToList(),
            });
        }
        #endregion


        #region GetById
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var allocation = await _context.ProjectAllocations
                .Include(pa => pa.Project)
                .Include(pa => pa.Student)
                .ThenInclude(s => s.Department)
                .Include(pa => pa.Faculty)
                .Include(pa => pa.Tasks)
                .ThenInclude(t => t.TaskStatus)
                .FirstOrDefaultAsync(pa => pa.Id == id);

            if (allocation == null) return NotFound(new ApiResponse<object>
            {
                Success = false,
                StatusCode = 404,
                Errors = new List<string> { "Allocation not found." }
            });

            if (IsStudent())
            {
                var currentUserId = GetCurrentUserId();
                if (!currentUserId.HasValue || allocation.StudentId != currentUserId.Value)
                    return StatusCode(403, new ApiResponse<object>
                    {
                        Success = false,
                        StatusCode = 403,
                        Errors = new List<string> { "Access denied." }
                    });
            }

            return Ok(new ApiResponse<AllocationResponseDto>
            {
                Success = true,
                Data = MapToDto(allocation)
            });
        }
        #endregion


        #region CreateAllocation
        [HttpPost]
        [Authorize(Roles = "Admin,Faculty")]
        public async Task<IActionResult> Create(CreateAllocationDto dto)
        {

            // 1. Check Project exists
            var projectExist = await _context.Projects.AnyAsync(p => p.Id == dto.ProjectId);

            if (!projectExist)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    StatusCode = 400,
                    Errors = new List<string> { $"Project with ID {dto.ProjectId} does not exist." }
                });
            }

            // 2. Check Student Exist
            var studentExist = await _context.Users.AnyAsync(s => s.Id == dto.StudentId);

            if (!studentExist)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    StatusCode = 400,
                    Errors = new List<string> { $"Student with ID {dto.StudentId} does not exist." }
                });
            }

            //3. Check Faculty Exist
            var facultyExist = await _context.Users.AnyAsync(f => f.Id == dto.FacultyId);

            if (!facultyExist)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    StatusCode = 400,
                    Errors = new List<string> { $"Faculty with ID {dto.FacultyId} does not exist." }
                });
            }

            //4. Check Already Allocated Project

            var alreadyAllocated = await _context.ProjectAllocations.AnyAsync(pa => pa.ProjectId == dto.ProjectId && pa.StudentId == dto.StudentId);

            if (alreadyAllocated)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    StatusCode = 400,
                    Errors = new List<string> { "This student is already allocated to this project." }
                });
            }

            //5. Create Allocation
            var allocation = new ProjectAllocation
            {
                ProjectId = dto.ProjectId,
                StudentId = dto.StudentId,
                FacultyId = dto.FacultyId,
                ProjectStartDate = dto.ProjectStartDate,
                ProjectEndDate = dto.ProjectEndDate,
                AssignedDate = DateTime.Now,
                CreatedAt = DateTime.Now
            };

            _context.ProjectAllocations.Add(allocation);
            await _context.SaveChangesAsync();

            //6. get created Allocation
            var created = await _context.ProjectAllocations
                .Include(pa => pa.Project)
                .Include(pa => pa.Student)
                .ThenInclude(s => s.Department)
                .Include(pa => pa.Faculty)
                .Include(pa => pa.Tasks)
                .FirstAsync(pa => pa.Id == allocation.Id);

            return CreatedAtAction(nameof(GetById), new { id = allocation.Id }, new ApiResponse<AllocationResponseDto>
            {
                Success = true,
                StatusCode = 201,
                Data = MapToDto(created),
                Message = "Project allocation created successfully."
            });
        }
        #endregion


        #region UpdateAllocation
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Faculty")]
        public async Task<IActionResult> Update(int id, UpdateAllocationDto dto)
        {
            var allocation = await _context.ProjectAllocations.FindAsync(id);
            if (allocation == null) return NotFound(new ApiResponse<object>
            {
                Success = false,
                StatusCode = 404,
                Errors = new List<string> { "Allocation not found." }
            });

            allocation.ProjectStartDate = dto.ProjectStartDate;
            allocation.ProjectEndDate = dto.ProjectEndDate;
            allocation.OverAllGrade = dto.OverAllGrade;
            allocation.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            var updated = await _context.ProjectAllocations
                .Include(pa => pa.Project)
                .Include(pa => pa.Student)
                .ThenInclude(s => s.Department)
                .Include(pa => pa.Faculty)
                .Include(pa => pa.Tasks)
                .FirstAsync(pa => pa.Id == allocation.Id);

            return Ok(new ApiResponse<AllocationResponseDto>
            {
                Success = true,
                Data = MapToDto(updated),
                Message = "Project allocation updated successfully."
            });
        }
        #endregion


        #region DeleteAllocation
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var allocation = await _context.ProjectAllocations.FindAsync(id);
            if (allocation == null) return NotFound(new ApiResponse<object>
            {
                Success = false,
                StatusCode = 404,
                Errors = new List<string> { "Allocation not found." }
            });

            _context.ProjectAllocations.Remove(allocation);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Project allocation deleted successfully."
            });
        }
        #endregion
    }
}
