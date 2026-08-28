namespace LittleWins.Application.UseCases.Completions.ApproveCompletion;

public sealed record ApproveCompletionResult(
    Guid CompletionId,
    Guid MemberId,
    int PointsAwarded);