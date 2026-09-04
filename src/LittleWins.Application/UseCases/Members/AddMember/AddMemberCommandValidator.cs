using FluentValidation;

namespace LittleWins.Application.UseCases.Members.AddMember;

public sealed class AddMemberCommandValidator
    : AbstractValidator<AddMemberCommand>
{
    private const int MaximumNameLength = 100;

    public AddMemberCommandValidator()
    {
        RuleFor(command => command.Name)
            .NotEmpty()
            .WithMessage("Member name is required.")
            .MaximumLength(MaximumNameLength)
            .WithMessage(
                $"Member name must not exceed {MaximumNameLength} characters.");

        RuleFor(command => command.Role)
            .IsInEnum()
            .WithMessage("Member role is invalid.");
    }
}