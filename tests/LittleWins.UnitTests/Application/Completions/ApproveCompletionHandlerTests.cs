using FluentAssertions;
using LittleWins.Application.UseCases.Completions.ApproveCompletion;
using LittleWins.Domain.Entities;
using LittleWins.Domain.Enums;
using LittleWins.UnitTests.Fakes;

namespace LittleWins.UnitTests.Application.Completions;

public class ApproveCompletionHandlerTests
{
    [Fact]
    public async Task HandleAsync_ParentApprovesCompletion_AwardsActivityPoints()
    {
        // Arrange
        var family = CreateFamily();
        var child = CreateChild(family);
        var parent = CreateParent(family);
        var activity = CreateActivity(family, child);
        var completion = CreateCompletion(activity, child);

        var completionRepository =
            new FakeActivityCompletionRepository(completion);

        var activityRepository =
            new FakeActivityRepository(activity);

        var memberRepository =
            new FakeMemberRepository(child, parent);

        var unitOfWork = new FakeUnitOfWork();

        var validator = new ApproveCompletionCommandValidator();

        var handler = new ApproveCompletionHandler(
            completionRepository,
            activityRepository,
            memberRepository,
            unitOfWork,
            validator);

        var command = new ApproveCompletionCommand(
            completion.Id,
            parent.Id);

        // Act
        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        // Assert
        result.CompletionId.Should().Be(completion.Id);
        result.MemberId.Should().Be(child.Id);
        result.PointsAwarded.Should().Be(activity.Points);

        completion.Status.Should().Be(CompletionStatus.Approved);
        completion.PointsAwarded.Should().BeTrue();
        completion.ApprovedByMemberId.Should().Be(parent.Id);

        child.Points.Should().Be(activity.Points);

        unitOfWork.SaveChangesCallCount.Should().Be(1);
    }

    [Fact]
    public async Task HandleAsync_CompletionDoesNotExist_ThrowsInvalidOperationException()
    {
        // Arrange
        var completionRepository =
            new FakeActivityCompletionRepository();

        var activityRepository =
            new FakeActivityRepository();

        var memberRepository =
            new FakeMemberRepository();

        var unitOfWork = new FakeUnitOfWork();

        var validator = new ApproveCompletionCommandValidator();

        var handler = new ApproveCompletionHandler(
            completionRepository,
            activityRepository,
            memberRepository,
            unitOfWork,
            validator);

        var command = new ApproveCompletionCommand(
            Guid.NewGuid(),
            Guid.NewGuid());

        // Act
        var act = () => handler.HandleAsync(
            command,
            CancellationToken.None);

        // Assert
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Completion was not found.");

        unitOfWork.SaveChangesCallCount.Should().Be(0);
    }

    [Fact]
    public async Task HandleAsync_ActivityDoesNotExist_ThrowsInvalidOperationException()
    {
        // Arrange
        var completion = new ActivityCompletion(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid());

        var completionRepository =
            new FakeActivityCompletionRepository(completion);

        var activityRepository =
            new FakeActivityRepository();

        var memberRepository =
            new FakeMemberRepository();

        var unitOfWork = new FakeUnitOfWork();

        var validator = new ApproveCompletionCommandValidator();

        var handler = new ApproveCompletionHandler(
            completionRepository,
            activityRepository,
            memberRepository,
            unitOfWork,
            validator);

        var command = new ApproveCompletionCommand(
            completion.Id,
            Guid.NewGuid());

        // Act
        var act = () => handler.HandleAsync(
            command,
            CancellationToken.None);

        // Assert
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Activity was not found.");

        unitOfWork.SaveChangesCallCount.Should().Be(0);
    }

    [Fact]
    public async Task HandleAsync_ApprovingMemberDoesNotExist_ThrowsInvalidOperationException()
    {
        // Arrange
        var family = CreateFamily();
        var child = CreateChild(family);
        var activity = CreateActivity(family, child);
        var completion = CreateCompletion(activity, child);

        var completionRepository =
            new FakeActivityCompletionRepository(completion);

        var activityRepository =
            new FakeActivityRepository(activity);

        var memberRepository =
            new FakeMemberRepository(child);

        var unitOfWork = new FakeUnitOfWork();

        var validator = new ApproveCompletionCommandValidator();

        var handler = new ApproveCompletionHandler(
            completionRepository,
            activityRepository,
            memberRepository,
            unitOfWork,
            validator);

        var command = new ApproveCompletionCommand(
            completion.Id,
            Guid.NewGuid());

        // Act
        var act = () => handler.HandleAsync(
            command,
            CancellationToken.None);

        // Assert
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Approving member was not found.");

        child.Points.Should().Be(0);
        unitOfWork.SaveChangesCallCount.Should().Be(0);
    }

    [Fact]
    public async Task HandleAsync_ApprovingMemberIsChild_ThrowsInvalidOperationException()
    {
        // Arrange
        var family = CreateFamily();
        var child = CreateChild(family);
        var activity = CreateActivity(family, child);
        var completion = CreateCompletion(activity, child);

        var completionRepository =
            new FakeActivityCompletionRepository(completion);

        var activityRepository =
            new FakeActivityRepository(activity);

        var memberRepository =
            new FakeMemberRepository(child);

        var unitOfWork = new FakeUnitOfWork();

        var validator = new ApproveCompletionCommandValidator();

        var handler = new ApproveCompletionHandler(
            completionRepository,
            activityRepository,
            memberRepository,
            unitOfWork,
            validator);

        var command = new ApproveCompletionCommand(
            completion.Id,
            child.Id);

        // Act
        var act = () => handler.HandleAsync(
            command,
            CancellationToken.None);

        // Assert
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Only a parent can approve a completion.");

        child.Points.Should().Be(0);
        completion.Status.Should().Be(CompletionStatus.Pending);
        completion.PointsAwarded.Should().BeFalse();
        unitOfWork.SaveChangesCallCount.Should().Be(0);
    }

    [Fact]
    public async Task HandleAsync_ApprovingMemberBelongsToDifferentFamily_ThrowsInvalidOperationException()
    {
        // Arrange
        var family = CreateFamily();
        var differentFamily = new Family("The Jones Family");

        var child = CreateChild(family);
        var parent = CreateParent(differentFamily);

        var activity = CreateActivity(family, child);
        var completion = CreateCompletion(activity, child);

        var completionRepository =
            new FakeActivityCompletionRepository(completion);

        var activityRepository =
            new FakeActivityRepository(activity);

        var memberRepository =
            new FakeMemberRepository(child, parent);

        var unitOfWork = new FakeUnitOfWork();

        var validator = new ApproveCompletionCommandValidator();

        var handler = new ApproveCompletionHandler(
            completionRepository,
            activityRepository,
            memberRepository,
            unitOfWork,
            validator);

        var command = new ApproveCompletionCommand(
            completion.Id,
            parent.Id);

        // Act
        var act = () => handler.HandleAsync(
            command,
            CancellationToken.None);

        // Assert
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Approving member does not belong to the family.");

        child.Points.Should().Be(0);
        completion.Status.Should().Be(CompletionStatus.Pending);
        completion.PointsAwarded.Should().BeFalse();
        unitOfWork.SaveChangesCallCount.Should().Be(0);
    }

    [Fact]
    public async Task HandleAsync_CompletionAlreadyApproved_DoesNotAwardPointsAgain()
    {
        // Arrange
        var family = CreateFamily();
        var child = CreateChild(family);
        var parent = CreateParent(family);
        var activity = CreateActivity(family, child);
        var completion = CreateCompletion(activity, child);

        completion.Approve(parent.Id);
        completion.MarkPointsAwarded();

        child.AwardPoints(activity.Points);

        var completionRepository =
            new FakeActivityCompletionRepository(completion);

        var activityRepository =
            new FakeActivityRepository(activity);

        var memberRepository =
            new FakeMemberRepository(child, parent);

        var unitOfWork = new FakeUnitOfWork();

        var validator = new ApproveCompletionCommandValidator();

        var handler = new ApproveCompletionHandler(
            completionRepository,
            activityRepository,
            memberRepository,
            unitOfWork,
            validator);

        var command = new ApproveCompletionCommand(
            completion.Id,
            parent.Id);

        var pointsBeforeSecondApproval = child.Points;

        // Act
        var act = () => handler.HandleAsync(
            command,
            CancellationToken.None);

        // Assert
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("*already been approved*");

        child.Points.Should().Be(pointsBeforeSecondApproval);
        completion.PointsAwarded.Should().BeTrue();
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

    private static Member CreateParent(Family family)
    {
        return new Member(
            family.Id,
            "Parent",
            MemberRole.Parent);
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

    private static ActivityCompletion CreateCompletion(
        Activity activity,
        Member child)
    {
        return new ActivityCompletion(
            activity.Id,
            child.Id,
            activity.FamilyId);
    }
}