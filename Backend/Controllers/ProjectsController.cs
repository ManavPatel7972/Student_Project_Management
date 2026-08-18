using Backend.Data;
using Backend.DTOs.Project;
using Backend.Enums;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProjectsController : BasicInfoController
    {
        private readonly ApplicationDbContext _context;

        public ProjectsController(ApplicationDbContext context)
        {
            _context = context;
        }

        private static ProjectResponseDto MapToDto(ProjectMaster p)
        {
            return new ProjectResponseDto
            {
                Id = p.Id,
                ProjectTitle = p.ProjectTitle,
                Description = p.Description,
                Status = p.Status,
                TotalAllocations = p.Allocations != null ? p.Allocations.Count : 0,
                CreatedAt = p.CreatedAt
            };
        }

        #region GetProject(Filter)
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? search, [FromQuery] ProjectStatus? status)
        {
            var query = _context.Projects
                .Include(p => p.Allocations)
                .AsQueryable();

            if (IsStudent())
            {
                var currentUserId = GetCurrentUserId();

                if (!currentUserId.HasValue)
                {
                    return Unauthorized(new ApiResponse<object>
                    {
                        Success = false,
                        StatusCode = 401,
                        Errors = new List<string> { "Unauthorized." }
                    });
                }

                query = query.Where(p => p.Allocations.Any(a => a.StudentId == currentUserId.Value));
            }


            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                query = query.Where(p => p.ProjectTitle.ToLower().Contains(s) || (p.Description != null && p.Description.ToLower().Contains(s)));
            }

            if (status.HasValue)
            {
                query = query.Where(p => p.Status == status.Value);
            }

            var projects = await query.OrderByDescending(p => p.CreatedAt).ToListAsync();

            return Ok(new ApiResponse<List<ProjectResponseDto>>
            {
                Success = true,
                Data = projects.Select(MapToDto).ToList()
            });
        }
        #endregion


        #region GetProjectById
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var project = await _context.Projects
                .Include(p => p.Allocations)
                .ThenInclude(pa => pa.Student)
                .Include(p => p.Allocations)
                .ThenInclude(pa => pa.Faculty)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    StatusCode = 404,
                    Errors = new List<string> { "Project not found." }
                });
            }

            if (IsStudent())
            {
                var currentUserId = GetCurrentUserId();
                if (!currentUserId.HasValue ||
                    !project.Allocations.Any(a => a.StudentId == currentUserId.Value))
                {
                    return StatusCode(403, new ApiResponse<object>
                    {
                        Success = false,
                        StatusCode = 403,
                        Errors = new List<string> { "Access denied." }
                    });
                }
            }

            return Ok(new ApiResponse<ProjectResponseDto>
            {
                Success = true,
                Data = MapToDto(project)
            });
        }

        #endregion


        #region CreateProject
        [HttpPost]
        [Authorize(Roles = "Admin,Faculty")]
        public async Task<IActionResult> Create(CreateProjectDto dto)
        {
            var project = new ProjectMaster
            {
                ProjectTitle = dto.ProjectTitle,
                Description = dto.Description,
                Status = dto.Status,
                CreatedAt = DateTime.UtcNow
            };

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = project.Id }, new ApiResponse<ProjectResponseDto>
            {
                Success = true,
                StatusCode = 201,
                Data = MapToDto(project),
                Message = "Project created successfully."
            });
        }

        #endregion


        #region UpdateProject(Put)
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Faculty")]
        public async Task<IActionResult> Update(int id, UpdateProjectDto dto)
        {
            var project = await _context.Projects.FindAsync(id);

            if (project == null) return NotFound(new ApiResponse<object>
            {
                Success = false,
                StatusCode = 404,
                Errors = new List<string> { "Project not found." }
            });

            project.ProjectTitle = dto.ProjectTitle;
            project.Description = dto.Description;
            project.Status = dto.Status;
            project.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return Ok(new ApiResponse<ProjectResponseDto>
            {
                Success = true,
                Data = MapToDto(project),
                Message = "Project updated successfully."
            });
        }
        #endregion


        #region UpdateStatusOnly
        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin,Faculty")]
        public async Task<IActionResult> UpdateStatus(int id, UpdateProjectStatusDto dto)
        {
            var project = await _context.Projects.FindAsync(id);

            if (project == null) return NotFound(new ApiResponse<object>
            {
                Success = false,
                StatusCode = 404,
                Errors = new List<string> { "Project not found." }
            });

            project.Status = dto.Status;
            project.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Project status updated.",
                Data = new { Status = project.Status.ToString() }
            });
        }
        #endregion


        #region DeleteProject
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var project = await _context.Projects.FindAsync(id);

            if (project == null) return NotFound(new ApiResponse<object>
            {
                Success = false,
                StatusCode = 404,
                Errors = new List<string> { "Project not found." }
            });

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Project deleted successfully."
            });
        }
        #endregion
    }
}
