using LittleWins.Domain.Entities;

namespace LittleWins.Application.Abstractions.Persistence;

public interface IActivityRepository
{
    Task<Activity?> GetByIdAsync(
        Guid activityId,
        CancellationToken cancellationToken);

    Task AddAsync(
        Activity activity,
        CancellationToken cancellationToken);
}