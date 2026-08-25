using FluentAssertions;
using LittleWins.Domain.Entities;
using LittleWins.Domain.Enums;

namespace LittleWins.UnitTests.Domain;

public class ActivityTests
{
    [Fact]
    public void Constructor_ValidDetails_CreatesActiveActivity()
    {
        // Arrange
        var familyId = Guid.NewGuid();
        var assignedToMemberId = Guid.NewGuid();

        // Act
        var activity = new Activity(
            familyId,
            "Clean bedroom",
            "Tidy the bedroom",
            "Chore",
            assignedToMemberId,
            10,
            null,
            true);

        // Assert
        activity.Id.Should().NotBe(Guid.Empty);
        activity.FamilyId.Should().Be(familyId);
        activity.Title.Should().Be("Clean bedroom");
        activity.Description.Should().Be("Tidy the bedroom");
        activity.Category.Should().Be("Chore");
        activity.AssignedToMemberId.Should().Be(assignedToMemberId);
        activity.Points.Should().Be(10);
        activity.RequiresApproval.Should().BeTrue();
        activity.Status.Should().Be(ActivityStatus.Active);
    }

    [Fact]
    public void Constructor_EmptyFamilyId_ThrowsArgumentException()
    {
        // Arrange
        var familyId = Guid.Empty;

        // Act
        var act = () => new Activity(
            familyId,
            "Clean bedroom",
            null,
            "Chore",
            Guid.NewGuid(),
            10,
            null,
            true);

        // Assert
        act.Should()
            .Throw<ArgumentException>()
            .WithMessage("*Family ID is required*");
    }

    [Fact]
    public void Constructor_EmptyTitle_ThrowsArgumentException()
    {
        // Arrange
        const string title = "";

        // Act
        var act = () => new Activity(
            Guid.NewGuid(),
            title,
            null,
            "Chore",
            Guid.NewGuid(),
            10,
            null,
            true);

        // Assert
        act.Should()
            .Throw<ArgumentException>()
            .WithMessage("*Activity title is required*");
    }

    [Fact]
    public void Constructor_EmptyCategory_ThrowsArgumentException()
    {
        // Arrange
        const string category = "";

        // Act
        var act = () => new Activity(
            Guid.NewGuid(),
            "Clean bedroom",
            null,
            category,
            Guid.NewGuid(),
            10,
            null,
            true);

        // Assert
        act.Should()
            .Throw<ArgumentException>()
            .WithMessage("*Activity category is required*");
    }

    [Fact]
    public void Constructor_EmptyAssignedMemberId_ThrowsArgumentException()
    {
        // Arrange
        var assignedToMemberId = Guid.Empty;

        // Act
        var act = () => new Activity(
            Guid.NewGuid(),
            "Clean bedroom",
            null,
            "Chore",
            assignedToMemberId,
            10,
            null,
            true);

        // Assert
        act.Should()
            .Throw<ArgumentException>()
            .WithMessage("*Assigned member ID is required*");
    }

    [Fact]
    public void Constructor_ZeroPoints_ThrowsArgumentException()
    {
        // Arrange
        const int points = 0;

        // Act
        var act = () => new Activity(
            Guid.NewGuid(),
            "Clean bedroom",
            null,
            "Chore",
            Guid.NewGuid(),
            points,
            null,
            true);

        // Assert
        act.Should()
            .Throw<ArgumentException>()
            .WithMessage("*Points must be greater than zero*");
    }

    [Fact]
    public void Constructor_NegativePoints_ThrowsArgumentException()
    {
        // Arrange
        const int points = -1;

        // Act
        var act = () => new Activity(
            Guid.NewGuid(),
            "Clean bedroom",
            null,
            "Chore",
            Guid.NewGuid(),
            points,
            null,
            true);

        // Assert
        act.Should()
            .Throw<ArgumentException>()
            .WithMessage("*Points must be greater than zero*");
    }
}