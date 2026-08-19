using Backend.DTOs.Department;
using FluentValidation;

namespace Backend.Validators.Department
{
    public class UpdateDepartmentDtoValidator : AbstractValidator<UpdateDepartmentDto>
    {
        public UpdateDepartmentDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Department Name is required.")
                .MaximumLength(100).WithMessage("Department Name cannot exceed 100 characters.");
        }
    }
}
