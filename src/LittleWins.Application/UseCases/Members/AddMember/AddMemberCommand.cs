using LittleWins.Domain.Enums;

namespace LittleWins.Application.UseCases.Members.AddMember;

public sealed record AddMemberCommand(
    Guid FamilyId,
    string Name,
    MemberRole Role);