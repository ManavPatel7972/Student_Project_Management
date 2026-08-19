using Backend.DTOs.ProjectAllocation;
using FluentValidation;

namespace Backend.Validators.ProjectAllocation
{
    public class CreateAllocationDtoValidator : AbstractValidator<CreateAllocationDto>
    {
        public CreateAllocationDtoValidator()
        {
            RuleFor(x => x.ProjectId)
                .GreaterThan(0).WithMessage("Project allocation requires a valid Project.");

            RuleFor(x => x.StudentId)
                .GreaterThan(0).WithMessage("Project allocation requires a valid Student.");

            RuleFor(x => x.FacultyId)
                .GreaterThan(0).WithMessage("Project allocation requires a valid Faculty.");

            RuleFor(x => x.ProjectStartDate)
                .NotEmpty().WithMessage("Start date is required.");

            RuleFor(x => x.ProjectEndDate)
                .NotEmpty().WithMessage("End date is required.")
                .GreaterThanOrEqualTo(x => x.ProjectStartDate).WithMessage("End date must be on or after start date.");
        }
    }
}
