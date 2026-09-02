using LittleWins.Application.Abstractions.Persistence;
using LittleWins.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LittleWins.Infrastructure.Persistence.Repositories;

public sealed class FamilyRepository : IFamilyRepository
{
    private readonly LittleWinsDbContext _context;

    public FamilyRepository(LittleWinsDbContext context)
    {
        _context = context;
    }

    public async Task<Family?> GetByIdAsync(
        Guid familyId,
        CancellationToken cancellationToken)
    {
        return await _context.Families
            .FirstOrDefaultAsync(
                family => family.Id == familyId,
                cancellationToken);
    }

    public async Task AddAsync(
        Family family,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(family);

        await _context.Families.AddAsync(
            family,
            cancellationToken);
    }
}