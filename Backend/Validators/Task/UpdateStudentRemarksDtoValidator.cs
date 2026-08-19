using Backend.DTOs.Task;
using FluentValidation;

namespace Backend.Validators.Task
{
    public class UpdateStudentRemarksDtoValidator : AbstractValidator<UpdateStudentRemarksDto>
    {
        public UpdateStudentRemarksDtoValidator()
        {
            RuleFor(x => x.StudentRemarks)
                .NotEmpty().WithMessage("Student remarks cannot be empty.");
        }
    }
}
