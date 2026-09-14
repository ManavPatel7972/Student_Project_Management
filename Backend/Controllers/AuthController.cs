using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Backend.Data;
using Backend.DTOs.Auth;
using Backend.Helpers;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly JwtHelper _jwtHelper;

        public AuthController(ApplicationDbContext context, JwtHelper jwtHelper)
        {
            _context = context;
            _jwtHelper = jwtHelper;
        }

        #region Register
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterRequestDto request)
        {
            try
            {
                if (await _context.Users.AnyAsync(u => u.Email == request.Email))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Errors = new List<string> { "Email address is already registered." }
                    });
                }

                int roleId = request.UserTypeId;

                string prefixForUserCode = request.UserTypeId switch
                {
                    1 => "ADM",
                    2 => "FAC",
                    3 => "STU",
                    _ => "USR"
                };

                string userCode = $"{prefixForUserCode}{DateTime.Now.Year}";

                var user = new Backend.Models.User
                {
                    UserTypeId = request.UserTypeId,
                    DepartmentId = request.DepartmentId,
                    FullName = request.FullName,
                    UserCode = userCode,
                    Email = request.Email,
                    PasswordHash = PasswordHelper.HashPassword(request.Password),
                    MobileNumber = request.MobileNumber,
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                var userRole = new Backend.Models.UserRole
                {
                    UserId = user.Id,
                    RoleId = roleId
                };

                _context.UserRoles.Add(userRole);
                await _context.SaveChangesAsync();

                var roleObj = await _context.Roles.FindAsync(roleId);
                var primaryRole = roleObj?.RoleName ?? "User";

                var token = _jwtHelper.GenerateToken(user, primaryRole);

                var response = new LoginResponseDto
                {
                    Token = token,
                    UserId = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    Role = primaryRole,
                    UserTypeId = user.UserTypeId,
                    UserCode = user.UserCode,
                    ProfilePicturePath = user.ProfilePicturePath
                };

                return Ok(new ApiResponse<LoginResponseDto>
                {
                    Success = true,
                    Data = response,
                    Message = "Registration successful."
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

        #region Login
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginRequestDto request)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.UserType)
                    .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                    .FirstOrDefaultAsync(u => u.Email == request.Email && u.IsActive);

                if (user == null || !PasswordHelper.VerifyPassword(request.Password, user.PasswordHash))
                {
                    return Unauthorized(new ApiResponse<object>
                    {
                        Success = false,
                        Errors = new List<string> { "Invalid email or password." }
                    });
                }

                var primaryRole = user.UserRoles.FirstOrDefault()?.Role.RoleName ?? "User";
                var token = _jwtHelper.GenerateToken(user, primaryRole);

                var response = new LoginResponseDto
                {
                    UserId = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    Role = primaryRole,
                    UserTypeId = user.UserTypeId,
                    UserCode = user.UserCode,
                    ProfilePicturePath = user.ProfilePicturePath,
                    Token = token,
                };

                return Ok(new ApiResponse<LoginResponseDto>
                {
                    Success = true,
                    Data = response,
                    Message = "Login successful."
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

        #region GetCurrentUser
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                Console.WriteLine("Authenticated USER ======" + User);
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                Console.WriteLine("userIdClaim =============" + userIdClaim);

                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized(new ApiResponse<object>
                    {
                        Success = false,
                        Errors = new List<string> { "Unauthorized." }
                    });
                }

                int userId = Convert.ToInt32(userIdClaim);

                var user = await _context
                .Users
                    .Include(u => u.UserType)
                    .Include(u => u.Department)
                    .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Errors = new List<string> { "User not found." }
                    });
                }

                var primaryRole = user.UserRoles.FirstOrDefault()?.Role.RoleName ?? "User";

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Data = new
                    {
                        user.Id,
                        user.FullName,
                        user.Email,
                        user.UserCode,
                        user.MobileNumber,
                        user.UserTypeId,
                        UserTypeName = user.UserType.UserTypeName,
                        DepartmentName = user.Department?.Name,
                        Role = primaryRole,
                        user.ProfilePicturePath
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

        #region ChangePassword
        [HttpPut("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized(new ApiResponse<object>
                    {
                        Success = false,
                        Errors = new List<string> { "Unauthorized." }
                    });
                }

                int userId = Convert.ToInt32(userIdClaim);

                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Errors = new List<string> { "User not found." }
                    });
                }

                if (!PasswordHelper.VerifyPassword(dto.CurrentPassword, user.PasswordHash))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        Success = false,
                        Errors = new List<string> { "Current password is incorrect." }
                    });
                }

                user.PasswordHash = PasswordHelper.HashPassword(dto.NewPassword);
                user.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Password updated successfully."
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

        #region Logout
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized(new ApiResponse<object>
                    {
                        Success = false,
                        StatusCode = StatusCodes.Status401Unauthorized,
                        Errors = new List<string> { "Unauthorized" },
                    });
                }

                var userId = Convert.ToInt32(userIdClaim);

                var user = await _context.Users.FindAsync(userId);

                if (user == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Errors = new List<string> { "User not found." }
                    });
                }

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Logout successful."
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
