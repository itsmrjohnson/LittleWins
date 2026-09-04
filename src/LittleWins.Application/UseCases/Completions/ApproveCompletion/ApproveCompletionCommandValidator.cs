using FluentValidation;

namespace LittleWins.Application.UseCases.Completions.ApproveCompletion;

public sealed class ApproveCompletionCommandValidator
    : AbstractValidator<ApproveCompletionCommand>
{
    public ApproveCompletionCommandValidator()
    {
        RuleFor(command => command.CompletionId)
            .NotEmpty()
            .WithMessage("Completion is required.");

        RuleFor(command => command.ApprovedByMemberId)
            .NotEmpty()
            .WithMessage("Approving member is required.");
    }
}