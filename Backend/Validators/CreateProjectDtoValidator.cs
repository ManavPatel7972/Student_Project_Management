using Backend.DTOs.Project;
using FluentValidation;

namespace Backend.Validators
{
    public class CreateProjectDtoValidator : AbstractValidator<CreateProjectDto>
    {
        public CreateProjectDtoValidator()
        {
            RuleFor(x => x.ProjectTitle)
                .NotEmpty().WithMessage("Project Title is required.")
                .MaximumLength(200).WithMessage("Project Title cannot exceed 200 characters.");
        }
    }
}
