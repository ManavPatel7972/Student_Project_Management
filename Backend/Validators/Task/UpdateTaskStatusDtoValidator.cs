using Backend.DTOs.Task;
using FluentValidation;

namespace Backend.Validators.Task
{
    public class UpdateTaskStatusDtoValidator : AbstractValidator<UpdateTaskStatusDto>
    {
        public UpdateTaskStatusDtoValidator()
        {
            RuleFor(x => x.TaskStatusId)
                .GreaterThan(0).WithMessage("Task Status is required.");
        }
    }
}
