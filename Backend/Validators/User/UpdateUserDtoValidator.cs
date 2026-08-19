using Backend.DTOs.User;
using FluentValidation;

namespace Backend.Validators.User
{
    public class UpdateUserDtoValidator : AbstractValidator<UpdateUserDto>
    {
        public UpdateUserDtoValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full Name is required.")
                .MaximumLength(150).WithMessage("Full Name cannot exceed 150 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("A valid email address is required.");

            RuleFor(x => x.MobileNumber)
                .NotEmpty().WithMessage("Mobile Number is required.")
                .MaximumLength(15).WithMessage("Mobile Number cannot exceed 15 characters.");

            RuleFor(x => x.UserTypeId)
                .GreaterThan(0).WithMessage("User Type is required.");

            RuleFor(x => x.RoleId)
                .GreaterThan(0).WithMessage("Role is required.");
        }
    }
}
