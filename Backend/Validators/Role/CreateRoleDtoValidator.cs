using Backend.DTOs.Role;
using FluentValidation;

namespace Backend.Validators.Role
{
    public class CreateRoleDtoValidator : AbstractValidator<CreateRoleDto>
    {
        public CreateRoleDtoValidator()
        {
            RuleFor(x => x.RoleName)
                .NotEmpty().WithMessage("Role Name is required.")
                .MaximumLength(100).WithMessage("Role Name cannot exceed 100 characters.");
        }
    }
}
