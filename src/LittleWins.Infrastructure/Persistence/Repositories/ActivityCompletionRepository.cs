using LittleWins.Application.Abstractions.Persistence;
using LittleWins.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LittleWins.Infrastructure.Persistence.Repositories;

public sealed class ActivityCompletionRepository : IActivityCompletionRepository
{
    private readonly LittleWinsDbContext _context;

    public ActivityCompletionRepository(LittleWinsDbContext context)
    {
        _context = context;
    }

    public async Task<ActivityCompletion?> GetByIdAsync(
        Guid completionId,
        CancellationToken cancellationToken)
    {
        return await _context.ActivityCompletions
            .FirstOrDefaultAsync(
                completion => completion.Id == completionId,
                cancellationToken);
    }

    public async Task AddAsync(
        ActivityCompletion completion,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(completion);

        await _context.ActivityCompletions.AddAsync(
            completion,
            cancellationToken);
    }
}