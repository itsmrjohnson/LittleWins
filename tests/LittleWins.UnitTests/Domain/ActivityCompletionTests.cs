using FluentAssertions;
using LittleWins.Domain.Entities;
using LittleWins.Domain.Enums;

namespace LittleWins.UnitTests.Domain;

public class ActivityCompletionTests
{
    [Fact]
    public void Constructor_ValidDetails_CreatesPendingCompletion()
    {
        // Arrange
        var activityId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var familyId = Guid.NewGuid();

        // Act
        var completion = new ActivityCompletion(
            activityId,
            memberId,
            familyId);

        // Assert
        completion.Id.Should().NotBe(Guid.Empty);
        completion.ActivityId.Should().Be(activityId);
        completion.MemberId.Should().Be(memberId);
        completion.FamilyId.Should().Be(familyId);
        completion.Status.Should().Be(CompletionStatus.Pending);
        completion.PointsAwarded.Should().BeFalse();
        completion.CompletedAt.Should().BeCloseTo(
            DateTime.UtcNow,
            TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Approve_PendingCompletion_SetsStatusToApproved()
    {
        // Arrange
        var completion = CreateCompletion();
        var parentId = Guid.NewGuid();

        // Act
        completion.Approve(parentId);

        // Assert
        completion.Status.Should().Be(CompletionStatus.Approved);
        completion.ApprovedByMemberId.Should().Be(parentId);
        completion.ApprovedAt.Should().NotBeNull();
    }

    [Fact]
    public void Approve_AlreadyApprovedCompletion_ThrowsInvalidOperationException()
    {
        // Arrange
        var completion = CreateCompletion();

        completion.Approve(Guid.NewGuid());

        // Act
        var act = () => completion.Approve(Guid.NewGuid());

        // Assert
        act.Should()
            .Throw<InvalidOperationException>()
            .WithMessage("*already been approved*");
    }

    [Fact]
    public void Approve_RejectedCompletion_ThrowsInvalidOperationException()
    {
        // Arrange
        var completion = CreateCompletion();

        completion.Reject();

        // Act
        var act = () => completion.Approve(Guid.NewGuid());

        // Assert
        act.Should()
            .Throw<InvalidOperationException>()
            .WithMessage("*rejected completion cannot be approved*");
    }

    [Fact]
    public void Reject_PendingCompletion_SetsStatusToRejected()
    {
        // Arrange
        var completion = CreateCompletion();

        // Act
        completion.Reject();

        // Assert
        completion.Status.Should().Be(CompletionStatus.Rejected);
    }

    [Fact]
    public void Reject_ApprovedCompletion_ThrowsInvalidOperationException()
    {
        // Arrange
        var completion = CreateCompletion();

        completion.Approve(Guid.NewGuid());

        // Act
        var act = () => completion.Reject();

        // Assert
        act.Should()
            .Throw<InvalidOperationException>()
            .WithMessage("*approved completion cannot be rejected*");
    }

    [Fact]
    public void MarkPointsAwarded_ApprovedCompletion_SetsPointsAwarded()
    {
        // Arrange
        var completion = CreateCompletion();

        completion.Approve(Guid.NewGuid());

        // Act
        completion.MarkPointsAwarded();

        // Assert
        completion.PointsAwarded.Should().BeTrue();
    }

    [Fact]
    public void MarkPointsAwarded_PendingCompletion_ThrowsInvalidOperationException()
    {
        // Arrange
        var completion = CreateCompletion();

        // Act
        var act = () => completion.MarkPointsAwarded();

        // Assert
        act.Should()
            .Throw<InvalidOperationException>()
            .WithMessage("*only be awarded for an approved completion*");
    }

    [Fact]
    public void MarkPointsAwarded_AlreadyAwardedCompletion_ThrowsInvalidOperationException()
    {
        // Arrange
        var completion = CreateCompletion();

        completion.Approve(Guid.NewGuid());
        completion.MarkPointsAwarded();

        // Act
        var act = () => completion.MarkPointsAwarded();

        // Assert
        act.Should()
            .Throw<InvalidOperationException>()
            .WithMessage("*already been awarded*");
    }

    [Fact]
    public void Constructor_EmptyActivityId_ThrowsArgumentException()
    {
        // Arrange
        var activityId = Guid.Empty;

        // Act
        var act = () => new ActivityCompletion(
            activityId,
            Guid.NewGuid(),
            Guid.NewGuid());

        // Assert
        act.Should()
            .Throw<ArgumentException>()
            .WithMessage("*Activity ID is required*");
    }

    [Fact]
    public void Constructor_EmptyMemberId_ThrowsArgumentException()
    {
        // Arrange
        var memberId = Guid.Empty;

        // Act
        var act = () => new ActivityCompletion(
            Guid.NewGuid(),
            memberId,
            Guid.NewGuid());

        // Assert
        act.Should()
            .Throw<ArgumentException>()
            .WithMessage("*Member ID is required*");
    }

    [Fact]
    public void Constructor_EmptyFamilyId_ThrowsArgumentException()
    {
        // Arrange
        var familyId = Guid.Empty;

        // Act
        var act = () => new ActivityCompletion(
            Guid.NewGuid(),
            Guid.NewGuid(),
            familyId);

        // Assert
        act.Should()
            .Throw<ArgumentException>()
            .WithMessage("*Family ID is required*");
    }

    private static ActivityCompletion CreateCompletion()
    {
        return new ActivityCompletion(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid());
    }
}