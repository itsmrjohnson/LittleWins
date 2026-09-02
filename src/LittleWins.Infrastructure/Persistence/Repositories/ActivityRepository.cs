using LittleWins.Application.Abstractions.Persistence;
using LittleWins.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LittleWins.Infrastructure.Persistence.Repositories;

public sealed class ActivityRepository : IActivityRepository
{
    private readonly LittleWinsDbContext _context;

    public ActivityRepository(LittleWinsDbContext context)
    {
        _context = context;
    }

    public async Task<Activity?> GetByIdAsync(
        Guid activityId,
        CancellationToken cancellationToken)
    {
        return await _context.Activities
            .FirstOrDefaultAsync(
                activity => activity.Id == activityId,
                cancellationToken);
    }

    public async Task AddAsync(
        Activity activity,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(activity);

        await _context.Activities.AddAsync(
            activity,
            cancellationToken);
    }
}