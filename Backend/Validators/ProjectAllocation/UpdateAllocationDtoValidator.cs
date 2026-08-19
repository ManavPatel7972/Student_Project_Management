using Backend.DTOs.ProjectAllocation;
using FluentValidation;

namespace Backend.Validators.ProjectAllocation
{
    public class UpdateAllocationDtoValidator : AbstractValidator<UpdateAllocationDto>
    {
        public UpdateAllocationDtoValidator()
        {
            RuleFor(x => x.ProjectStartDate)
                .NotEmpty().WithMessage("Start date is required.");

            RuleFor(x => x.ProjectEndDate)
                .NotEmpty().WithMessage("End date is required.")
                .GreaterThanOrEqualTo(x => x.ProjectStartDate).WithMessage("End date must be on or after start date.");
        }
    }
}
