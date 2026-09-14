using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Data;
using Backend.DTOs.Role;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class RolesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RolesController(ApplicationDbContext context)
        {
            _context = context;
        }

        private static RoleResponseDto MapToDto(Role r)
        {
            return new RoleResponseDto
            {
                Id = r.Id,
                RoleName = r.RoleName,
                Description = r.Description,
                UserCount = r.UserRoles != null ? r.UserRoles.Count : 0
            };
        }

        #region GetAllRoles
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var roles = await _context.Roles
                    .Include(r => r.UserRoles)
                    .ToListAsync();

                return Ok(new ApiResponse<List<RoleResponseDto>>
                {
                    Success = true,
                    Data = roles.Select(MapToDto).ToList(),
                    Message = "All Roles Fetched Successfully."
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

        #region GetRoleByID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var role = await _context.Roles
                    .Include(r => r.UserRoles)
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (role == null) return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    StatusCode = 404,
                    Errors = new List<string> { "Role not found." }
                });

                return Ok(new ApiResponse<RoleResponseDto>
                {
                    Success = true,
                    Data = MapToDto(role),
                    Message = "Role By Id Fetched Successfully."
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

        #region CreateRole
        [HttpPost]
        public async Task<IActionResult> Create(CreateRoleDto dto)
        {
            try
            {
                if (await _context.Roles.AnyAsync(r => r.RoleName == dto.RoleName))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        StatusCode = 400,
                        Errors = new List<string> { "Role name already exists." }
                    });
                }

                var role = new Role
                {
                    RoleName = dto.RoleName,
                    Description = dto.Description
                };

                _context.Roles.Add(role);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetById), new { id = role.Id }, new ApiResponse<RoleResponseDto>
                {
                    Success = true,
                    StatusCode = 201,
                    Data = MapToDto(role),
                    Message = "Role created successfully."
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

        #region UpdateRole
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateRoleDto dto)
        {
            try
            {
                var role = await _context.Roles.FindAsync(id);
                if (role == null) return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    StatusCode = 404,
                    Errors = new List<string> { "Role not found." }
                });

                role.RoleName = dto.RoleName;
                role.Description = dto.Description;
                await _context.SaveChangesAsync();

                var updatedRole = await _context.Roles
                    .Include(r => r.UserRoles)
                    .FirstOrDefaultAsync(r => r.Id == id);

                return Ok(new ApiResponse<RoleResponseDto>
                {
                    Success = true,
                    Data = MapToDto(updatedRole ?? role),
                    Message = "Role updated successfully."
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

        #region DeleteRole
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var role = await _context.Roles.FindAsync(id);

                if (role == null) return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    StatusCode = 404,
                    Errors = new List<string> { "Role not found." }
                });

                var rolePermission = await _context.RolePermissions
                    .Where(rp => rp.RoleId == id)
                    .ToListAsync();

                _context.RolePermissions.RemoveRange(rolePermission);

                var userRoles = await _context.UserRoles
                    .Where(ur => ur.RoleId == id)
                    .ToListAsync();

                _context.UserRoles.RemoveRange(userRoles);

                _context.Roles.Remove(role);

                await _context.SaveChangesAsync();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Role deleted successfully."
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
