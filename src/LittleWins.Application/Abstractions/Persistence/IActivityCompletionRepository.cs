using LittleWins.Domain.Entities;

namespace LittleWins.Application.Abstractions.Persistence;

public interface IActivityCompletionRepository
{
    Task<ActivityCompletion?> GetByIdAsync(
        Guid completionId,
        CancellationToken cancellationToken);

    Task AddAsync(
        ActivityCompletion completion,
        CancellationToken cancellationToken);
}