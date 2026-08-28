using LittleWins.Application.Abstractions.Persistence;
using LittleWins.Domain.Entities;

namespace LittleWins.Application.UseCases.Members.AddMember;

public sealed class AddMemberHandler
{
    private readonly IFamilyRepository _familyRepository;
    private readonly IMemberRepository _memberRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddMemberHandler(
        IFamilyRepository familyRepository,
        IMemberRepository memberRepository,
        IUnitOfWork unitOfWork)
    {
        _familyRepository = familyRepository;
        _memberRepository = memberRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<AddMemberResult> HandleAsync(
        AddMemberCommand command,
        CancellationToken cancellationToken)
    {
        var family = await _familyRepository.GetByIdAsync(
            command.FamilyId,
            cancellationToken);

        if (family is null)
        {
            throw new InvalidOperationException(
                "Family was not found.");
        }

        var member = new Member(
            family.Id,
            command.Name,
            command.Role);

        family.AddMember(member);

        await _memberRepository.AddAsync(
            member,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return new AddMemberResult(
            member.Id,
            member.FamilyId,
            member.Name);
    }
}