using APBD6.DTOs;
using FluentValidation;

namespace APBD6.Validators;

public class CreateAnimalRequestValidator : AbstractValidator<CreateAnimalRequest>
{
    public CreateAnimalRequestValidator()
    {
        RuleFor(e => e.Name).MaximumLength(50).NotNull();
        RuleFor(e => e.Description).MaximumLength(100);
        RuleFor(e => e.Category).MaximumLength(50).NotNull();
        RuleFor(e => e.Area).MaximumLength(50).NotNull();
    }
}