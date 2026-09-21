using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Backend.Data;
using Backend.DTOs.User;
using Backend.Helpers;
using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IProfileImageService _profileImageService;

        public UsersController(ApplicationDbContext context, IProfileImageService profileImageService)
        {
            _context = context;
            _profileImageService = profileImageService;
        }

        //Not Learning Now AutoMapper so..... using static Mapper....
        private static UserResponseDto MapToDto(User u)
        {
            var userRole = u.UserRoles.FirstOrDefault();
            return new UserResponseDto
            {
                Id = u.Id,
                UserTypeId = u.UserTypeId,
                UserTypeName = u.UserType != null ? u.UserType.UserTypeName : string.Empty,
                DepartmentId = u.DepartmentId,
                DepartmentName = u.Department != null ? u.Department.Name : null,
                RoleId = userRole != null ? userRole.RoleId : 0,
                RoleName = userRole != null && userRole.Role != null ? userRole.Role.RoleName : string.Empty,
                FullName = u.FullName,
                UserCode = u.UserCode,
                Email = u.Email,
                MobileNumber = u.MobileNumber,
                ProfilePicturePath = u.ProfilePicturePath,
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt
            };
        }

        #region GetAll(Filter)
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? search,
            [FromQuery] int? userTypeId,
            [FromQuery] int? departmentId,
            [FromQuery] int? roleId,
            [FromQuery] bool? isActive
            )
        {
            try
            {
                var query = _context.Users
                    .Include(u => u.UserType)
                    .Include(u => u.Department)
                    .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(search))
                {
                    var s = search.Trim().ToLower();
                    query = query.Where(u =>
                        u.FullName.ToLower().Contains(s) ||
                        u.Email.ToLower().Contains(s) ||
                        (u.UserCode != null && u.UserCode.ToLower().Contains(s)) ||
                        u.MobileNumber.Contains(s));
                }

                if (userTypeId != null) query = query.Where(u => u.UserTypeId == userTypeId);
                if (departmentId != null) query = query.Where(u => u.DepartmentId == departmentId);
                if (roleId != null) query = query.Where(u => u.UserRoles.Any(ur => ur.RoleId == roleId));
                if (isActive.HasValue) query = query.Where(u => u.IsActive == isActive);

                var users = await query.OrderByDescending(u => u.CreatedAt).ToListAsync();

                List<UserResponseDto> dtos = new();

                foreach (var user in users)
                {
                    dtos.Add(MapToDto(user));
                }

                return Ok(new ApiResponse<List<UserResponseDto>>
                {
                    Success = true,
                    Data = dtos,
                    Message = "Records Fetched Sussessfully.."
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

        #region GetById
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.UserType)
                    .Include(u => u.Department)
                    .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (user == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        StatusCode = 404,
                        Errors = new List<string> { "User not found." }
                    });
                }

                return Ok(new ApiResponse<UserResponseDto>
                {
                    Success = true,
                    Data = MapToDto(user),
                    Message = "User By Id Fetched Successfully."
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

        #region CreateUser
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Create(CreateUserDto dto)
        {
            try
            {
                if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        StatusCode = 400,
                        Errors = new List<string> { "Email address already registered." }
                    });
                }

                var user = new User
                {
                    UserTypeId = dto.UserTypeId,
                    DepartmentId = dto.DepartmentId,
                    FullName = dto.FullName,
                    UserCode = dto.UserCode,
                    Email = dto.Email,
                    PasswordHash = PasswordHelper.HashPassword(dto.Password),
                    MobileNumber = dto.MobileNumber,
                    IsActive = dto.IsActive,
                    CreatedAt = DateTime.Now
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Assign Role
                var userRole = new UserRole
                {
                    UserId = user.Id,
                    RoleId = dto.RoleId
                };

                _context.UserRoles.Add(userRole);
                await _context.SaveChangesAsync();

                var createdUser = await _context.Users
                    .Include(u => u.UserType)
                    .Include(u => u.Department)
                    .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                    .FirstAsync(u => u.Id == user.Id);

                return CreatedAtAction(nameof(GetById), new { id = user.Id }, new ApiResponse<UserResponseDto>
                {
                    Success = true,
                    StatusCode = 201,
                    Data = MapToDto(createdUser),
                    Message = "User created successfully."
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

        #region Update
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, UpdateUserDto dto)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.UserRoles)
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (user == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        StatusCode = 404,
                        Errors = new List<string> { "User not found." }
                    });
                }

                user.FullName = dto.FullName;
                user.UserCode = dto.UserCode;
                user.Email = dto.Email;
                user.MobileNumber = dto.MobileNumber;
                user.UserTypeId = dto.UserTypeId;
                user.DepartmentId = dto.DepartmentId;
                user.IsActive = dto.IsActive;
                user.UpdatedAt = DateTime.Now;

                // Update role (Delete Multiple)
                _context.UserRoles.RemoveRange(user.UserRoles);
                _context.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = dto.RoleId });

                await _context.SaveChangesAsync();

                var updatedUser = await _context.Users
                    .Include(u => u.UserType)
                    .Include(u => u.Department)
                    .Include(u => u.UserRoles)
                        .ThenInclude(ur => ur.Role)
                    .FirstAsync(u => u.Id == user.Id);

                return Ok(new ApiResponse<UserResponseDto>
                {
                    Success = true,
                    Data = MapToDto(updatedUser),
                    Message = "User updated successfully."
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

        #region UpdateParialUserDetails
        [HttpPatch("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdatePartial(int id, UpdateUserpatchDto dto)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.UserRoles)
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (user == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        StatusCode = 404,
                        Errors = new List<string> { "User Not Found" }
                    });
                }

                //Update only provided details
                if (dto.FullName != null) user.FullName = dto.FullName;
                if (dto.UserCode != null) user.UserCode = dto.UserCode;
                if (dto.Email != null) user.Email = dto.Email;
                if (dto.MobileNumber != null) user.MobileNumber = dto.MobileNumber;
                if (dto.DepartmentId.HasValue) user.DepartmentId = dto.DepartmentId.Value;
                if (dto.UserTypeId.HasValue) user.UserTypeId = dto.UserTypeId.Value;
                if (dto.isActive.HasValue) user.IsActive = dto.isActive.Value;

                user.UpdatedAt = DateTime.Now;

                if (dto.RoleId.HasValue)
                {
                    _context.UserRoles.RemoveRange(user.UserRoles);

                    _context.UserRoles.Add(new UserRole
                    {
                        UserId = user.Id,
                        RoleId = dto.RoleId.Value,
                    });
                }

                await _context.SaveChangesAsync();

                var updatedUser = await _context.Users
                    .Include(u => u.UserType)
                    .Include(u => u.Department)
                    .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                    .FirstAsync(u => u.Id == user.Id);

                return Ok(new ApiResponse<UserResponseDto>
                {
                    Success = true,
                    Data = MapToDto(updatedUser),
                    Message = "User updated successfully."
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

        #region SoftDelete
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);

                if (user == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        StatusCode = 404,
                        Errors = new List<string> { "User not found." }
                    });
                }

                user.IsDeleted = true;
                user.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "User deleted successfully."
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

        #region UploadPhoto
        [HttpPost("{id}/upload-photo")]
        [Authorize]
        public async Task<IActionResult> UploadPhoto(int id, IFormFile file)
        {
            try
            {
                if (!CanManageProfileImage(id)) return Forbid();

                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (user == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        StatusCode = 404,
                        Message = "User Not Found",
                        Errors = new List<string> { "User Not Found" }
                    });
                }

                if (file == null || file.Length == 0)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        StatusCode = 400,
                        Errors = new List<string> { "Please select a profile image." }
                    });
                }

                var allowedExtension = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

                if (!allowedExtension.Contains(extension))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        StatusCode = 400,
                        Errors = new List<string>
                        {
                            "Only JPG, JPEG, PNG and WEBP images are allowed."
                        }
                    });
                }

                if (file.Length > 5 * 1024 * 1024)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        StatusCode = 400,
                        Errors = new List<string>
                        {
                            "Profile image cannot be larger than 5 MB."
                        }
                    });
                }

                var uploadedImage = await _profileImageService.UploadAsync(file, user.Id);

                user.ProfilePicturePath = uploadedImage.ImageUrl;
                user.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    StatusCode = 200,
                    Message = "Profile photo uploaded successfully.",
                    Data = new
                    {
                        photoUrl = uploadedImage.ImageUrl
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR ================" + ex.Message);
                Console.WriteLine("Depth =====>>>>>>>>>>>>>>>>" + ex);
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

        #region DeletePhoto
        [HttpDelete("{id}/profile-photo")]
        [Authorize]
        public async Task<IActionResult> DeleteProfilePhoto(int id)
        {
            try
            {
                if (!CanManageProfileImage(id)) return Forbid();

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

                if (user == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        StatusCode = 404,
                        Errors = new List<string>
                        {
                             "User not found."
                        }
                    });
                }

                if (string.IsNullOrEmpty(user.ProfilePicturePath))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        StatusCode = 400,
                        Errors = new List<string>
                        {
                               "User does not have a profile photo."
                        }
                    });
                }

                if (user.ProfilePicturePath.Contains("res.cloudinary.com", StringComparison.OrdinalIgnoreCase))
                {
                    await _profileImageService.DeleteAsync(user.ProfilePicturePath);
                }

                user.ProfilePicturePath = null;
                user.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    StatusCode = 200,
                    Message = "Profile photo deleted successfully."
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

        private bool CanManageProfileImage(int userId)
        {
            //var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //return User.IsInRole("Admin") || currentUserId == userId.ToString();
            return true;
        }
    }
}
