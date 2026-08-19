using Backend.DTOs.Project;
using FluentValidation;

namespace Backend.Validators.Project
{
    public class UpdateProjectStatusDtoValidator : AbstractValidator<UpdateProjectStatusDto>
    {
        public UpdateProjectStatusDtoValidator()
        {
            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("A valid project status must be selected.");
        }
    }
}
