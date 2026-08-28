namespace LittleWins.Application.UseCases.Completions.ApproveCompletion;

public sealed record ApproveCompletionCommand(
    Guid CompletionId,
    Guid ApprovedByMemberId);