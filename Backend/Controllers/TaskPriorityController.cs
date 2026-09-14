using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Data;
using Backend.DTOs.TaskPriority;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/task-priorities")]
    [Authorize]
    public class TaskPriorityController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TaskPriorityController(ApplicationDbContext context)
        {
            _context = context;
        }

        #region GetAllPriorities
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var priorities = await _context.TaskPriorities.ToListAsync();

                var dtos = priorities.Select(p => new TaskPriorityResponseDto
                {
                    Id = p.Id,
                    TaskPriorityName = p.TaskPriorityName,
                    TaskPriorityCssClass = p.TaskPriorityCssClass
                }).ToList();

                return Ok(new ApiResponse<List<TaskPriorityResponseDto>>
                {
                    Success = true,
                    Data = dtos,
                    Message = "All Prioritys Fetched Successfully."
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

        #region GetPriorityById
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var priority = await _context.TaskPriorities
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (priority == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        StatusCode = 404,
                        Errors = new List<string>
                        {
                            $"Task priority with ID {id} not found."
                        }
                    });
                }

                var dto = new TaskPriorityResponseDto
                {
                    Id = priority.Id,
                    TaskPriorityName = priority.TaskPriorityName,
                    TaskPriorityCssClass = priority.TaskPriorityCssClass
                };

                return Ok(new ApiResponse<TaskPriorityResponseDto>
                {
                    Success = true,
                    StatusCode = 200,
                    Data = dto,
                    Message = "TaskPriority By Id fetched Successfully."
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

        #region CreatePriority
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CreateTaskPriorityDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.TaskPriorityName))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        StatusCode = 400,
                        Errors = new List<string>
                        {
                            "Task priority name is required."
                        }
                    });
                }

                // Check duplicate priority name
                var exists = await _context.TaskPriorities
                    .AnyAsync(p => p.TaskPriorityName.ToLower() == dto.TaskPriorityName.Trim().ToLower());

                if (exists)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        StatusCode = 400,
                        Errors = new List<string>
                        {
                            $"Task priority '{dto.TaskPriorityName}' already exists."
                        }
                    });
                }

                var priority = new TaskPriorityLookup
                {
                    TaskPriorityName = dto.TaskPriorityName.Trim(),
                    TaskPriorityCssClass = dto.TaskPriorityCssClass?.Trim() ?? string.Empty
                };

                _context.TaskPriorities.Add(priority);
                await _context.SaveChangesAsync();

                var created = new TaskPriorityResponseDto
                {
                    Id = priority.Id,
                    TaskPriorityName = priority.TaskPriorityName,
                    TaskPriorityCssClass = priority.TaskPriorityCssClass
                };

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = priority.Id },
                    new ApiResponse<TaskPriorityResponseDto>
                    {
                        Success = true,
                        StatusCode = 201,
                        Data = created,
                        Message = "Task priority created successfully."
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

        #region UpdatePriority
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, UpdateTaskPriorityLookupDto dto)
        {
            try
            {
                var priority = await _context.TaskPriorities
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (priority == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        StatusCode = 404,
                        Errors = new List<string>
                        {
                            $"Task priority with ID {id} not found."
                        }
                    });
                }

                if (string.IsNullOrWhiteSpace(dto.TaskPriorityName))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        StatusCode = 400,
                        Errors = new List<string>
                        {
                            "Task priority name is required."
                        }
                    });
                }

                // Check duplicate name
                // Ignore the current record being updated
                var duplicateExists = await _context.TaskPriorities
                    .AnyAsync(p =>
                        p.Id != id &&
                        p.TaskPriorityName.ToLower() ==
                        dto.TaskPriorityName.Trim().ToLower());

                if (duplicateExists)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        StatusCode = 400,
                        Errors = new List<string>
                        {
                            $"Task priority '{dto.TaskPriorityName}' already exists."
                        }
                    });
                }

                priority.TaskPriorityName = dto.TaskPriorityName.Trim();
                priority.TaskPriorityCssClass = dto.TaskPriorityCssClass?.Trim() ?? string.Empty;

                await _context.SaveChangesAsync();

                var responseDto = new TaskPriorityResponseDto
                {
                    Id = priority.Id,
                    TaskPriorityName = priority.TaskPriorityName,
                    TaskPriorityCssClass = priority.TaskPriorityCssClass
                };

                return Ok(new ApiResponse<TaskPriorityResponseDto>
                {
                    Success = true,
                    StatusCode = 200,
                    Data = responseDto,
                    Message = "Task priority updated successfully."
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

        #region DeletePriority
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var priority = await _context.TaskPriorities
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (priority == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        StatusCode = 404,
                        Errors = new List<string>
                        {
                            $"Task priority with ID {id} not found."
                        }
                    });
                }

                // Check this priority is used by any task
                var isUsed = await _context.Tasks.AnyAsync(t => t.TaskPriorityId == id);

                if (isUsed)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        StatusCode = 400,
                        Errors = new List<string>
                        {
                            "This task priority cannot be deleted because it is currently being used by one or more tasks."
                        }
                    });
                }

                _context.TaskPriorities.Remove(priority);
                await _context.SaveChangesAsync();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    StatusCode = 200,
                    Message = "Task priority deleted successfully."
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
