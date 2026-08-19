using Backend.DTOs.Project;
using FluentValidation;

namespace Backend.Validators.Project
{
    public class UpdateProjectDtoValidator : AbstractValidator<UpdateProjectDto>
    {
        public UpdateProjectDtoValidator()
        {
            RuleFor(x => x.ProjectTitle)
                .NotEmpty().WithMessage("Project Title is required.")
                .MaximumLength(200).WithMessage("Project Title cannot exceed 200 characters.");
        }
    }
}
