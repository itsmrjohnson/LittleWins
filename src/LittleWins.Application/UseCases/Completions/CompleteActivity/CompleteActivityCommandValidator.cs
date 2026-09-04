using FluentValidation;

namespace LittleWins.Application.UseCases.Completions.CompleteActivity;

public sealed class CompleteActivityCommandValidator
    : AbstractValidator<CompleteActivityCommand>
{
    public CompleteActivityCommandValidator()
    {
        RuleFor(command => command.ActivityId)
            .NotEmpty()
            .WithMessage("Activity is required.");

        RuleFor(command => command.MemberId)
            .NotEmpty()
            .WithMessage("Member is required.");
    }
}