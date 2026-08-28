namespace LittleWins.Application.UseCases.Activities.CreateActivity;

public sealed record CreateActivityCommand(
    Guid FamilyId,
    string Title,
    string? Description,
    string Category,
    Guid AssignedToMemberId,
    int Points,
    DateTime? DueDate,
    bool RequiresApproval);