using LittleWins.Application.Abstractions.Persistence;

namespace LittleWins.Infrastructure.Persistence;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly LittleWinsDbContext _context;

    public UnitOfWork(LittleWinsDbContext context)
    {
        _context = context;
    }

    public Task<int> SaveChangesAsync(
        CancellationToken cancellationToken)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}