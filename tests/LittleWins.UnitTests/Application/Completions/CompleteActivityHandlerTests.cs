using FluentAssertions;
using LittleWins.Application.UseCases.Completions.CompleteActivity;
using LittleWins.Domain.Entities;
using LittleWins.Domain.Enums;
using LittleWins.UnitTests.Fakes;

namespace LittleWins.UnitTests.Application.Completions;

public class CompleteActivityHandlerTests
{
    [Fact]
    public async Task HandleAsync_AssignedMember_CompletesActivity()
    {
        // Arrange
        var family = CreateFamily();
        var child = CreateChild(family);
        var activity = CreateActivity(family, child);

        var activityRepository = new FakeActivityRepository(activity);
        var memberRepository = new FakeMemberRepository(child);
        var completionRepository = new FakeActivityCompletionRepository();
        var unitOfWork = new FakeUnitOfWork();

        var handler = new CompleteActivityHandler(
            activityRepository,
            memberRepository,
            completionRepository,
            unitOfWork);

        var command = new CompleteActivityCommand(
            activity.Id,
            child.Id);

        // Act
        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        // Assert
        result.CompletionId.Should().NotBe(Guid.Empty);
        result.ActivityId.Should().Be(activity.Id);
        result.MemberId.Should().Be(child.Id);
        result.Status.Should().Be(CompletionStatus.Pending.ToString());

        completionRepository.AddedCompletion.Should().NotBeNull();
        completionRepository.AddedCompletion!.ActivityId.Should().Be(activity.Id);
        completionRepository.AddedCompletion.MemberId.Should().Be(child.Id);
        completionRepository.AddedCompletion.FamilyId.Should().Be(family.Id);
        completionRepository.AddedCompletion.Status.Should().Be(CompletionStatus.Pending);

        unitOfWork.SaveChangesCallCount.Should().Be(1);
    }

    [Fact]
    public async Task HandleAsync_ActivityDoesNotExist_ThrowsInvalidOperationException()
    {
        // Arrange
        var activityRepository = new FakeActivityRepository();
        var memberRepository = new FakeMemberRepository();
        var completionRepository = new FakeActivityCompletionRepository();
        var unitOfWork = new FakeUnitOfWork();

        var handler = new CompleteActivityHandler(
            activityRepository,
            memberRepository,
            completionRepository,
            unitOfWork);

        var command = new CompleteActivityCommand(
            Guid.NewGuid(),
            Guid.NewGuid());

        // Act
        var act = () => handler.HandleAsync(
            command,
            CancellationToken.None);

        // Assert
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Activity was not found.");

        completionRepository.AddedCompletion.Should().BeNull();
        unitOfWork.SaveChangesCallCount.Should().Be(0);
    }

    [Fact]
    public async Task HandleAsync_MemberDoesNotExist_ThrowsInvalidOperationException()
    {
        // Arrange
        var family = CreateFamily();
        var child = CreateChild(family);
        var activity = CreateActivity(family, child);

        var activityRepository = new FakeActivityRepository(activity);
        var memberRepository = new FakeMemberRepository();
        var completionRepository = new FakeActivityCompletionRepository();
        var unitOfWork = new FakeUnitOfWork();

        var handler = new CompleteActivityHandler(
            activityRepository,
            memberRepository,
            completionRepository,
            unitOfWork);

        var command = new CompleteActivityCommand(
            activity.Id,
            child.Id);

        // Act
        var act = () => handler.HandleAsync(
            command,
            CancellationToken.None);

        // Assert
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Member was not found.");

        completionRepository.AddedCompletion.Should().BeNull();
        unitOfWork.SaveChangesCallCount.Should().Be(0);
    }

    [Fact]
    public async Task HandleAsync_MemberBelongsToDifferentFamily_ThrowsInvalidOperationException()
    {
        // Arrange
        var family = CreateFamily();
        var differentFamily = new Family("The Jones Family");

        var assignedChild = CreateChild(family);
        var differentChild = CreateChild(differentFamily);

        var activity = CreateActivity(family, assignedChild);

        var activityRepository = new FakeActivityRepository(activity);
        var memberRepository = new FakeMemberRepository(differentChild);
        var completionRepository = new FakeActivityCompletionRepository();
        var unitOfWork = new FakeUnitOfWork();

        var handler = new CompleteActivityHandler(
            activityRepository,
            memberRepository,
            completionRepository,
            unitOfWork);

        var command = new CompleteActivityCommand(
            activity.Id,
            differentChild.Id);

        // Act
        var act = () => handler.HandleAsync(
            command,
            CancellationToken.None);

        // Assert
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Member does not belong to the activity family.");

        completionRepository.AddedCompletion.Should().BeNull();
        unitOfWork.SaveChangesCallCount.Should().Be(0);
    }

    [Fact]
    public async Task HandleAsync_MemberIsNotAssignedToActivity_ThrowsInvalidOperationException()
    {
        // Arrange
        var family = CreateFamily();
        var assignedChild = CreateChild(family);
        var differentChild = CreateChild(family);

        var activity = CreateActivity(family, assignedChild);

        var activityRepository = new FakeActivityRepository(activity);
        var memberRepository = new FakeMemberRepository(differentChild);
        var completionRepository = new FakeActivityCompletionRepository();
        var unitOfWork = new FakeUnitOfWork();

        var handler = new CompleteActivityHandler(
            activityRepository,
            memberRepository,
            completionRepository,
            unitOfWork);

        var command = new CompleteActivityCommand(
            activity.Id,
            differentChild.Id);

        // Act
        var act = () => handler.HandleAsync(
            command,
            CancellationToken.None);

        // Assert
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Member is not assigned to this activity.");

        completionRepository.AddedCompletion.Should().BeNull();
        unitOfWork.SaveChangesCallCount.Should().Be(0);
    }

    private static Family CreateFamily()
    {
        return new Family("The Smith Family");
    }

    private static Member CreateChild(Family family)
    {
        return new Member(
            family.Id,
            "Alex",
            MemberRole.Child);
    }

    private static Activity CreateActivity(
        Family family,
        Member child)
    {
        return new Activity(
            family.Id,
            "Clean bedroom",
            null,
            "Chore",
            child.Id,
            10,
            null,
            true);
    }
}