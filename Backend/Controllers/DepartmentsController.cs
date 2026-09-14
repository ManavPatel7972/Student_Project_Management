using Backend.Data;
using Backend.DTOs.Department;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DepartmentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DepartmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        #region GetAllDepartment
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var departments = await _context.Departments
                    .Include(d => d.Users)
                    .Select(d => new DepartmentResponseDto
                    {
                        Id = d.Id,
                        Name = d.Name,
                        UserCount = d.Users != null ? d.Users.Count : 0
                    })
                    .ToListAsync();

                return Ok(new ApiResponse<List<DepartmentResponseDto>>
                {
                    Success = true,
                    Data = departments,
                    Message = "All Department fetched successfully."
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

        #region GetDepartmentByID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var department = await _context.Departments
                    .Include(d => d.Users)
                    .FirstOrDefaultAsync(d => d.Id == id);

                if (department == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        StatusCode = 404,
                        Errors = new List<string> { "Department not found." }
                    });
                }

                return Ok(new ApiResponse<DepartmentResponseDto>
                {
                    Success = true,
                    Data = new DepartmentResponseDto
                    {
                        Id = department.Id,
                        Name = department.Name,
                        UserCount = department.Users != null ? department.Users.Count : 0
                    }
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

        #region CreateDepartment
        [HttpPost]
        [Authorize(Roles = "Admin")]
        //[Authorize(Policy ="AdminOnly")]
        public async Task<IActionResult> Create(CreateDepartmentDto dto)
        {
            try
            {
                if (await _context.Departments.AnyAsync(d => d.Name == dto.Name))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        StatusCode = 400,
                        Errors = new List<string> { "Department already exists." }
                    });
                }

                var department = new Department { Name = dto.Name };
                _context.Departments.Add(department);
                await _context.SaveChangesAsync();

                var responseDto = new DepartmentResponseDto
                {
                    Id = department.Id,
                    Name = department.Name,
                    UserCount = 0
                };

                //Controll go to GetById using CreatedAtAction
                return CreatedAtAction(nameof(GetById), new { id = department.Id }, new ApiResponse<DepartmentResponseDto>
                {
                    Success = true,
                    StatusCode = 201,
                    Data = responseDto,
                    Message = "Department created successfully."
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

        #region UpdateDepartment
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, UpdateDepartmentDto dto)
        {
            try
            {
                var department = await _context.Departments
                    .Include(d => d.Users)
                    .FirstOrDefaultAsync(d => d.Id == id);

                if (department == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        StatusCode = 404,
                        Errors = new List<string> { "Department not found." }
                    });
                }

                department.Name = dto.Name;
                await _context.SaveChangesAsync();

                var responseDto = new DepartmentResponseDto
                {
                    Id = id,
                    Name = dto.Name,
                    UserCount = department.Users != null ? department.Users.Count : 0,
                };

                return Ok(new ApiResponse<DepartmentResponseDto>
                {
                    Success = true,
                    Data = responseDto,
                    Message = "Department updated successfully."
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

        #region DeleteDepartment
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var department = await _context.Departments.FindAsync(id);
                if (department == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        StatusCode = 404,
                        Errors = new List<string> { "Department not found." }
                    });
                }

                _context.Departments.Remove(department);
                await _context.SaveChangesAsync();
                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Department deleted successfully."
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
