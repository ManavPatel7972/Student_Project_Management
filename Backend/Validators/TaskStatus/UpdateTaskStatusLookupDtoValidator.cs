using Backend.DTOs.TaskStatus;
using FluentValidation;

namespace Backend.Validators.TaskStatus
{
    public class UpdateTaskStatusLookupDtoValidator : AbstractValidator<UpdateTaskStatusLookupDto>
    {
        public UpdateTaskStatusLookupDtoValidator()
        {
            RuleFor(x => x.TaskStatusName)
                .NotEmpty().WithMessage("Task Status Name is required.")
                .MaximumLength(100).WithMessage("Task Status Name cannot exceed 100 characters.");

            RuleFor(x => x.TaskStatusCssClass)
                .NotEmpty().WithMessage("Task Status CSS Class is required.")
                .MaximumLength(100).WithMessage("Task Status CSS Class cannot exceed 100 characters.");
        }
    }
}
