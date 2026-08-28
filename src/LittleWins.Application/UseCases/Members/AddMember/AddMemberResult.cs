namespace LittleWins.Application.UseCases.Members.AddMember;

public sealed record AddMemberResult(
    Guid MemberId,
    Guid FamilyId,
    string Name);