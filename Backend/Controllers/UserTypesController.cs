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

        #endregion
    }
}
