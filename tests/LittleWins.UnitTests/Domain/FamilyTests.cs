using FluentAssertions;
using LittleWins.Domain.Entities;
using LittleWins.Domain.Enums;

namespace LittleWins.UnitTests.Domain;

public class FamilyTests
{
    [Fact]
    public void Constructor_ValidName_CreatesFamily()
    {
        // Arrange
        const string familyName = "The Smith Family";

        // Act
        var family = new Family(familyName);

        // Assert
        family.Id.Should().NotBe(Guid.Empty);
        family.Name.Should().Be(familyName);
        family.CreatedAt.Should().BeCloseTo(
            DateTime.UtcNow,
            TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Constructor_EmptyName_ThrowsArgumentException()
    {
        // Arrange
        const string familyName = "";

        // Act
        var act = () => new Family(familyName);

        // Assert
        act.Should()
            .Throw<ArgumentException>()
            .WithMessage("*Family name is required*");
    }

    [Fact]
    public void AddMember_MemberBelongsToFamily_AddsMember()
    {
        // Arrange
        var family = new Family("The Smith Family");
        var member = new Member(
            family.Id,
            "Alex",
            MemberRole.Child);

        // Act
        family.AddMember(member);

        // Assert
        family.Members.Should().ContainSingle()
            .Which.Should().Be(member);
    }

    [Fact]
    public void AddMember_MemberBelongsToDifferentFamily_ThrowsInvalidOperationException()
    {
        // Arrange
        var family = new Family("The Smith Family");
        var differentFamily = new Family("The Jones Family");

        var member = new Member(
            differentFamily.Id,
            "Alex",
            MemberRole.Child);

        // Act
        var act = () => family.AddMember(member);

        // Assert
        act.Should()
            .Throw<InvalidOperationException>()
            .WithMessage("*does not belong to this family*");
    }

    [Fact]
    public void AddActivity_ActivityBelongsToFamily_AddsActivity()
    {
        // Arrange
        var family = new Family("The Smith Family");

        var activity = new Activity(
            family.Id,
            "Clean bedroom",
            "Tidy the bedroom",
            "Chore",
            Guid.NewGuid(),
            10,
            null,
            true);

        // Act
        family.AddActivity(activity);

        // Assert
        family.Activities.Should().ContainSingle()
            .Which.Should().Be(activity);
    }

    [Fact]
    public void AddActivity_ActivityBelongsToDifferentFamily_ThrowsInvalidOperationException()
    {
        // Arrange
        var family = new Family("The Smith Family");
        var differentFamily = new Family("The Jones Family");

        var activity = new Activity(
            differentFamily.Id,
            "Clean bedroom",
            null,
            "Chore",
            Guid.NewGuid(),
            10,
            null,
            true);

        // Act
        var act = () => family.AddActivity(activity);

        // Assert
        act.Should()
            .Throw<InvalidOperationException>()
            .WithMessage("*does not belong to this family*");
    }
}