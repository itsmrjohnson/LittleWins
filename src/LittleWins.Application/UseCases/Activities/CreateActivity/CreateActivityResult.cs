namespace LittleWins.Application.UseCases.Activities.CreateActivity;

public sealed record CreateActivityResult(
    Guid ActivityId,
    Guid FamilyId,
    string Title,
    Guid AssignedToMemberId,
    int Points);