using LittleWins.Domain.Entities;

namespace LittleWins.Application.Abstractions.Persistence;

public interface IFamilyRepository
{
    Task<Family?> GetByIdAsync(
        Guid familyId,
        CancellationToken cancellationToken);

    Task AddAsync(
        Family family,
        CancellationToken cancellationToken);
}