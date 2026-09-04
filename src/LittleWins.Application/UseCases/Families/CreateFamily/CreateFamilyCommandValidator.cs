using FluentValidation;

namespace LittleWins.Application.UseCases.Families.CreateFamily;

public sealed class CreateFamilyCommandValidator
    : AbstractValidator<CreateFamilyCommand>
{
    private const int MaximumNameLength = 100;

    public CreateFamilyCommandValidator()
    {
        RuleFor(command => command.Name)
            .NotEmpty()
            .WithMessage("Family name is required.")
            .MaximumLength(MaximumNameLength)
            .WithMessage(
                $"Family name must not exceed {MaximumNameLength} characters.");
    }
}