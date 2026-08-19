using Backend.DTOs.Task;
using FluentValidation;

namespace Backend.Validators.Task
{
    public class UpdateEarnedScoreDtoValidator : AbstractValidator<UpdateEarnedScoreDto>
    {
        public UpdateEarnedScoreDtoValidator()
        {
            RuleFor(x => x.EarnedScore)
                .InclusiveBetween(0, 100).WithMessage("Earned Score must be between 0 and 100.");
        }
    }
}
