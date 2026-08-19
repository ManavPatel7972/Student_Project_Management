using Backend.DTOs.User;
using FluentValidation;

namespace Backend.Validators.User
{
    public class UpdateUserpatchDtoValidator : AbstractValidator<UpdateUserpatchDto>
    {
        public UpdateUserpatchDtoValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full Name cannot be empty.")
                .MaximumLength(150).WithMessage("Full Name cannot exceed 150 characters.")
                .When(x => x.FullName != null);

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email cannot be empty.")
                .EmailAddress().WithMessage("A valid email address is required.")
                .When(x => x.Email != null);

            RuleFor(x => x.MobileNumber)
                .NotEmpty().WithMessage("Mobile Number cannot be empty.")
                .MaximumLength(15).WithMessage("Mobile Number cannot exceed 15 characters.")
                .When(x => x.MobileNumber != null);
        }
    }
}
