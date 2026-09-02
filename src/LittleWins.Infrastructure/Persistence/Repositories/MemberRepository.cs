using LittleWins.Application.Abstractions.Persistence;
using LittleWins.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LittleWins.Infrastructure.Persistence.Repositories;

public sealed class MemberRepository : IMemberRepository
{
    private readonly LittleWinsDbContext _context;

    public MemberRepository(LittleWinsDbContext context)
    {
        _context = context;
    }

    public async Task<Member?> GetByIdAsync(
        Guid memberId,
        CancellationToken cancellationToken)
    {
        return await _context.Members
            .FirstOrDefaultAsync(
                member => member.Id == memberId,
                cancellationToken);
    }

    public async Task AddAsync(
        Member member,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(member);

        await _context.Members.AddAsync(
            member,
            cancellationToken);
    }
}