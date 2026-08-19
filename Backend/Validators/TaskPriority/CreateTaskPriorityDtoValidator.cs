using Backend.DTOs.TaskPriority;
using FluentValidation;

namespace Backend.Validators.TaskPriority
{
    public class CreateTaskPriorityDtoValidator : AbstractValidator<CreateTaskPriorityDto>
    {
        public CreateTaskPriorityDtoValidator()
        {
            RuleFor(x => x.TaskPriorityName)
                .NotEmpty().WithMessage("Task Priority Name is required.")
                .MaximumLength(100).WithMessage("Task Priority Name cannot exceed 100 characters.");

            RuleFor(x => x.TaskPriorityCssClass)
                .NotEmpty().WithMessage("Task Priority CSS Class is required.")
                .MaximumLength(100).WithMessage("Task Priority CSS Class cannot exceed 100 characters.");
        }
    }
}
