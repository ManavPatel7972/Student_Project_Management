using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Data;
using Backend.DTOs.Permission;
using Backend.DTOs.Role;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class PermissionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PermissionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        #region GetAllPermissionMatrix
        [HttpGet]
        public async Task<IActionResult> GetMatrix()
        {
            var roles = await _context.Roles.Include(r => r.UserRoles).ToListAsync();
            var permissions = await _context.Permissions.ToListAsync();
            var rolePermissions = await _context.RolePermissions.ToListAsync();

            var result = new RolePermissionMatrixDto
            {

                Roles = roles.Select(r => new RoleResponseDto
                {
                    Id = r.Id,
                    RoleName = r.RoleName,
                    Description = r.Description,
                    UserCount = r.UserRoles != null ? r.UserRoles.Count : 0
                }).ToList(),


                Permissions = permissions.Select(p => new PermissionResponseDto
                {
                    Id = p.Id,
                    PermissionName = p.PermissionName,
                    Description = p.Description
                }).ToList(),


                RolePermissions = rolePermissions.Select(rp => new RolePermissionItemDto
                {
                    RoleId = rp.RoleId,
                    PermissionId = rp.PermissionId,
                    IsGranted = rp.IsGranted
                }).ToList()
            };

            return Ok(new ApiResponse<RolePermissionMatrixDto>
            {
                Success = true,
                Data = result,
                Message = "Permission Fetched Successfully."
            }
            );
        }
        #endregion


        #region TogglePermission
        [HttpPut("toggle")]
        public async Task<IActionResult> TogglePermission(UpdateRolePermissionDto dto)
        {

            var rp = await _context.RolePermissions
                .FirstOrDefaultAsync(x => x.RoleId == dto.RoleId && x.PermissionId == dto.PermissionId);

            if (rp == null)
            {
                rp = new RolePermission
                {
                    RoleId = dto.RoleId,
                    PermissionId = dto.PermissionId,
                    IsGranted = dto.IsGranted
                };

                _context.RolePermissions.Add(rp);
            }
            else
            {
                rp.IsGranted = dto.IsGranted;
            }

            await _context.SaveChangesAsync();
            return Ok(new ApiResponse<object>
            { 
                Success = true, 
                Message = "Permission updated successfully." 
            });
        }
        #endregion
    }
}
