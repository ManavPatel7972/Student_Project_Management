using Backend.DTOs.Permission;
using FluentValidation;

namespace Backend.Validators.Permission
{
    public class UpdateRolePermissionDtoValidator : AbstractValidator<UpdateRolePermissionDto>
    {
        public UpdateRolePermissionDtoValidator()
        {
            RuleFor(x => x.RoleId)
                .GreaterThan(0).WithMessage("Valid Role ID is required.");

            RuleFor(x => x.PermissionId)
                .GreaterThan(0).WithMessage("Valid Permission ID is required.");
        }
    }
}
