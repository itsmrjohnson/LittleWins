using FluentValidation;

namespace LittleWins.Application.UseCases.Activities.CreateActivity;

public sealed class CreateActivityCommandValidator
    : AbstractValidator<CreateActivityCommand>
{
    private const int MaximumTitleLength = 200;
    private const int MaximumDescriptionLength = 1000;
    private const int MaximumCategoryLength = 100;

    public CreateActivityCommandValidator()
    {
        RuleFor(command => command.Title)
            .NotEmpty()
            .WithMessage("Activity title is required.")
            .MaximumLength(MaximumTitleLength)
            .WithMessage(
                $"Activity title must not exceed {MaximumTitleLength} characters.");

        RuleFor(command => command.Description)
            .MaximumLength(MaximumDescriptionLength)
            .WithMessage(
                $"Activity description must not exceed {MaximumDescriptionLength} characters.");

        RuleFor(command => command.Category)
            .NotEmpty()
            .WithMessage("Activity category is required.")
            .MaximumLength(MaximumCategoryLength)
            .WithMessage(
                $"Activity category must not exceed {MaximumCategoryLength} characters.");

        RuleFor(command => command.AssignedToMemberId)
            .NotEmpty()
            .WithMessage("Assigned member is required.");

        RuleFor(command => command.Points)
            .GreaterThan(0)
            .WithMessage("Activity points must be greater than zero.");

        RuleFor(command => command.DueDate)
            .GreaterThan(DateTime.UtcNow)
            .When(command => command.DueDate.HasValue)
            .WithMessage("Activity due date must be in the future.");
    }
}