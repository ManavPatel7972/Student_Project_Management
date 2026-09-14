using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Data;
using Backend.DTOs.TaskStatus;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/task-statuses")]
    [Authorize]
    public class TaskStatusController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TaskStatusController(ApplicationDbContext context)
        {
            _context = context;
        }

        #region GetAllStatus
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var statuses = await _context.TaskStatuses.ToListAsync();

                var dtos = statuses.Select(s => new TaskStatusResponseDto
                {
                    Id = s.Id,
                    TaskStatusName = s.TaskStatusName,
                    TaskStatusCssClass = s.TaskStatusCssClass
                }).ToList();

                return Ok(new ApiResponse<List<TaskStatusResponseDto>>
                {
                    Success = true,
                    Data = dtos
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

        #region GetStatusById
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var status = await _context.TaskStatuses
                    .FirstOrDefaultAsync(s => s.Id == id);

                if (status == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        StatusCode = 404,
                        Errors = new List<string> { $"Task status with ID {id} not found." }
                    });
                }

                var dto = new TaskStatusResponseDto
                {
                    Id = status.Id,
                    TaskStatusName = status.TaskStatusName,
                    TaskStatusCssClass = status.TaskStatusCssClass,
                };

                return Ok(new ApiResponse<TaskStatusResponseDto>
                {
                    Success = true,
                    Data = dto,
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

        #region CreateTaskStatus
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CreateTaskStatusDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.TaskStatusName))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        StatusCode = 400,
                        Errors = new List<string> { "Task status name is required." }
                    });
                }

                var exist = await _context.TaskStatuses
                    .AnyAsync(ts => ts.TaskStatusName.ToLower() == dto.TaskStatusName.ToLower());

                if (exist)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        StatusCode = 400,
                        Errors = new List<string> { $"Task status '{dto.TaskStatusName}' already exists." }
                    });
                }

                var status = new TaskStatusLookup
                {
                    TaskStatusName = dto.TaskStatusName.Trim(),
                    TaskStatusCssClass = dto.TaskStatusCssClass?.Trim() ?? string.Empty
                };

                _context.TaskStatuses.Add(status);
                await _context.SaveChangesAsync();

                var created = new TaskStatusResponseDto
                {
                    Id = status.Id,
                    TaskStatusName = status.TaskStatusName,
                    TaskStatusCssClass = status.TaskStatusCssClass,
                };

                return CreatedAtAction(nameof(GetById), new { id = created.Id }, new ApiResponse<TaskStatusResponseDto>
                {
                    Success = true,
                    StatusCode = 201,
                    Data = created,
                    Message = "Task status created successfully.",
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

        #region UpdateStatus
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, UpdateTaskStatusLookupDto dto)
        {
            try
            {
                var status = await _context.TaskStatuses
                    .FirstOrDefaultAsync(s => s.Id == id);

                if (status == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        StatusCode = 404,
                        Errors = new List<string>
                        {
                            $"Task status with ID {id} not found."
                        }
                    });
                }

                if (string.IsNullOrWhiteSpace(dto.TaskStatusName))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        StatusCode = 400,
                        Errors = new List<string>
                        {
                            "Task status name is required."
                        }
                    });
                }

                // Check duplicate name
                var duplicateExists = await _context.TaskStatuses
                    .AnyAsync(s =>
                        s.Id != id &&
                        s.TaskStatusName.ToLower() ==
                        dto.TaskStatusName.Trim().ToLower());

                if (duplicateExists)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        StatusCode = 400,
                        Errors = new List<string>
                        {
                            $"Task status '{dto.TaskStatusName}' already exists."
                        }
                    });
                }

                status.TaskStatusName = dto.TaskStatusName.Trim();
                status.TaskStatusCssClass =
                    dto.TaskStatusCssClass?.Trim() ?? string.Empty;

                await _context.SaveChangesAsync();

                var responseDto = new TaskStatusResponseDto
                {
                    Id = status.Id,
                    TaskStatusName = status.TaskStatusName,
                    TaskStatusCssClass = status.TaskStatusCssClass
                };

                return Ok(new ApiResponse<TaskStatusResponseDto>
                {
                    Success = true,
                    StatusCode = 200,
                    Data = responseDto,
                    Message = "Task status updated successfully."
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

        #region DeleteStatus
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var status = await _context.TaskStatuses
                    .FirstOrDefaultAsync(s => s.Id == id);

                if (status == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        StatusCode = 404,
                        Errors = new List<string>
                        {
                            $"Task status with ID {id} not found."
                        }
                    });
                }

                // Check this status is used by tasks
                var isUsed = await _context.Tasks
                    .AnyAsync(t => t.TaskStatusId == id);

                if (isUsed)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        StatusCode = 400,
                        Errors = new List<string>
                        {
                            "This task status cannot be deleted because it is currently being used by one or more tasks."
                        }
                    });
                }

                _context.TaskStatuses.Remove(status);
                await _context.SaveChangesAsync();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    StatusCode = 200,
                    Message = "Task status deleted successfully."
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
