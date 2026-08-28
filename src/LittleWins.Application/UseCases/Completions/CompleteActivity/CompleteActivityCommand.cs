namespace LittleWins.Application.UseCases.Completions.CompleteActivity;

public sealed record CompleteActivityCommand(
    Guid ActivityId,
    Guid MemberId);