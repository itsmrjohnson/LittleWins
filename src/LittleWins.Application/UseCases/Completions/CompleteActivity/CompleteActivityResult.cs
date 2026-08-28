namespace LittleWins.Application.UseCases.Completions.CompleteActivity;

public sealed record CompleteActivityResult(
    Guid CompletionId,
    Guid ActivityId,
    Guid MemberId,
    string Status);