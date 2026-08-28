using LittleWins.Application.Abstractions.Persistence;
using LittleWins.Domain.Entities;

namespace LittleWins.UnitTests.Fakes;

public sealed class FakeActivityRepository : IActivityRepository
{
    private readonly List<Activity> _activities = [];

    public FakeActivityRepository(params Activity[] activities)
    {
        _activities.AddRange(activities);
    }

    public Activity? AddedActivity { get; private set; }

    public Task<Activity?> GetByIdAsync(
        Guid activityId,
        CancellationToken cancellationToken)
    {
        var activity = _activities
            .SingleOrDefault(activity => activity.Id == activityId);

        return Task.FromResult(activity);
    }

    public Task AddAsync(
        Activity activity,
        CancellationToken cancellationToken)
    {
        AddedActivity = activity;
        _activities.Add(activity);

        return Task.CompletedTask;
    }
}