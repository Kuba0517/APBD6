using APBD6.DTOs;
using FluentValidation;

namespace APBD6.Validators;

public class UpdateAnimalRequestValidator : AbstractValidator<UpdateAnimalRequest>
{
    public UpdateAnimalRequestValidator()
    {
        RuleFor(e => e.Name).MaximumLength(50);
        RuleFor(e => e.Description).MaximumLength(100);
        RuleFor(e => e.Category).MaximumLength(50);
        RuleFor(e => e.Area).MaximumLength(50);
    }
}