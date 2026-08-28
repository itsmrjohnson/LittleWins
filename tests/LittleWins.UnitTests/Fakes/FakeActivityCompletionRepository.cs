using LittleWins.Application.Abstractions.Persistence;
using LittleWins.Domain.Entities;

namespace LittleWins.UnitTests.Fakes;

public sealed class FakeActivityCompletionRepository
    : IActivityCompletionRepository
{
    private readonly List<ActivityCompletion> _completions = [];

    public FakeActivityCompletionRepository(
        params ActivityCompletion[] completions)
    {
        _completions.AddRange(completions);
    }

    public ActivityCompletion? AddedCompletion { get; private set; }

    public Task<ActivityCompletion?> GetByIdAsync(
        Guid completionId,
        CancellationToken cancellationToken)
    {
        var completion = _completions
            .SingleOrDefault(
                completion => completion.Id == completionId);

        return Task.FromResult(completion);
    }

    public Task AddAsync(
        ActivityCompletion completion,
        CancellationToken cancellationToken)
    {
        AddedCompletion = completion;
        _completions.Add(completion);

        return Task.CompletedTask;
    }
}