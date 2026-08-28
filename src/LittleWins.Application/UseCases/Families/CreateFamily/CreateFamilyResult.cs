namespace LittleWins.Application.UseCases.Families.CreateFamily;

public sealed record CreateFamilyResult(
    Guid FamilyId,
    string Name);