using LittleWins.Application.Abstractions.Persistence;
using LittleWins.Domain.Entities;

namespace LittleWins.UnitTests.Fakes;

public sealed class FakeFamilyRepository : IFamilyRepository
{
    private readonly List<Family> _families = [];

    public FakeFamilyRepository(params Family[] families)
    {
        _families.AddRange(families);
    }

    public Family? AddedFamily { get; private set; }

    public Task<Family?> GetByIdAsync(
        Guid familyId,
        CancellationToken cancellationToken)
    {
        var family = _families
            .SingleOrDefault(family => family.Id == familyId);

        return Task.FromResult(family);
    }

    public Task AddAsync(
        Family family,
        CancellationToken cancellationToken)
    {
        AddedFamily = family;
        _families.Add(family);

        return Task.CompletedTask;
    }
}