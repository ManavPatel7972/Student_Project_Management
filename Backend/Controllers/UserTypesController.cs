using Backend.Data;
using Backend.DTOs.UserType;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/user-types")]
    [Authorize]
    public class UserTypesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserTypesController(ApplicationDbContext context)
        {
            _context = context;
        }

        #region GetAllUserTypes
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var userTypes = await _context.UserTypes.ToListAsync();
                var dtos = userTypes.Select(ut => new UserTypeResponseDto
                {
                    Id = ut.Id,
                    UserTypeName = ut.UserTypeName,
                    Description = ut.Description
                }).ToList();

                return Ok(new ApiResponse<List<UserTypeResponseDto>>
                {
                    Success = true,
                    Data = dtos,
                    Message = "All UserTypes Fetched Successfully."
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
