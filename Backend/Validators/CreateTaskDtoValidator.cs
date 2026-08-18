using Backend.DTOs.Task;
using FluentValidation;

namespace Backend.Validators
{
    public class CreateTaskDtoValidator : AbstractValidator<CreateTaskDto>
    {
        public CreateTaskDtoValidator()
        {
            RuleFor(x => x.ProjectAllocationId)
                .GreaterThan(0).WithMessage("Project Allocation is required.");

            RuleFor(x => x.TaskTitle)
                .NotEmpty().WithMessage("Task Title is required.")
                .MaximumLength(200).WithMessage("Task Title cannot exceed 200 characters.");

            RuleFor(x => x.TaskStatusId)
                .GreaterThan(0).WithMessage("Task Status is required.");

            RuleFor(x => x.TaskPriorityId)
                .GreaterThan(0).WithMessage("Task Priority is required.");

            RuleFor(x => x.AssignedScore)
                .InclusiveBetween(0, 100).WithMessage("Assigned Score must be between 0 and 100.");
        }
    }
}
