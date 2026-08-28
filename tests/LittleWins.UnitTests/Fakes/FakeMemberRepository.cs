using LittleWins.Application.Abstractions.Persistence;
using LittleWins.Domain.Entities;

namespace LittleWins.UnitTests.Fakes;

public sealed class FakeMemberRepository : IMemberRepository
{
    private readonly List<Member> _members = [];

    public FakeMemberRepository(params Member[] members)
    {
        _members.AddRange(members);
    }

    public Member? AddedMember { get; private set; }

    public Task<Member?> GetByIdAsync(
        Guid memberId,
        CancellationToken cancellationToken)
    {
        var member = _members
            .SingleOrDefault(member => member.Id == memberId);

        return Task.FromResult(member);
    }

    public Task AddAsync(
        Member member,
        CancellationToken cancellationToken)
    {
        AddedMember = member;
        _members.Add(member);

        return Task.CompletedTask;
    }
}