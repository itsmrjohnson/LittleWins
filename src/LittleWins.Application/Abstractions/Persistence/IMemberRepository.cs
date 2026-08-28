using LittleWins.Domain.Entities;

namespace LittleWins.Application.Abstractions.Persistence;

public interface IMemberRepository
{
    Task<Member?> GetByIdAsync(
        Guid memberId,
        CancellationToken cancellationToken);

    Task AddAsync(
        Member member,
        CancellationToken cancellationToken);
}