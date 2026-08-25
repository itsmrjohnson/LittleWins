using FluentAssertions;
using LittleWins.Domain.Entities;
using LittleWins.Domain.Enums;

namespace LittleWins.UnitTests.Domain;

public class MemberTests
{
    [Fact]
    public void Constructor_ValidDetails_CreatesMemberWithZeroPoints()
    {
        // Arrange
        var familyId = Guid.NewGuid();
        const string memberName = "Alex";

        // Act
        var member = new Member(
            familyId,
            memberName,
            MemberRole.Child);

        // Assert
        member.Id.Should().NotBe(Guid.Empty);
        member.FamilyId.Should().Be(familyId);
        member.Name.Should().Be(memberName);
        member.Role.Should().Be(MemberRole.Child);
        member.Points.Should().Be(0);
    }

    [Fact]
    public void Constructor_EmptyName_ThrowsArgumentException()
    {
        // Arrange
        var familyId = Guid.NewGuid();

        // Act
        var act = () => new Member(
            familyId,
            "",
            MemberRole.Child);

        // Assert
        act.Should()
            .Throw<ArgumentException>()
            .WithMessage("*Member name is required*");
    }

    [Fact]
    public void Constructor_EmptyFamilyId_ThrowsArgumentException()
    {
        // Arrange
        var familyId = Guid.Empty;

        // Act
        var act = () => new Member(
            familyId,
            "Alex",
            MemberRole.Child);

        // Assert
        act.Should()
            .Throw<ArgumentException>()
            .WithMessage("*Family ID is required*");
    }

    [Fact]
    public void AwardPoints_PositiveAmount_IncreasesPoints()
    {
        // Arrange
        var member = new Member(
            Guid.NewGuid(),
            "Alex",
            MemberRole.Child);

        const int points = 10;

        // Act
        member.AwardPoints(points);

        // Assert
        member.Points.Should().Be(points);
    }

    [Fact]
    public void AwardPoints_AdditionalPoints_IncreasesExistingBalance()
    {
        // Arrange
        var member = new Member(
            Guid.NewGuid(),
            "Alex",
            MemberRole.Child);

        member.AwardPoints(10);

        const int additionalPoints = 5;

        // Act
        member.AwardPoints(additionalPoints);

        // Assert
        member.Points.Should().Be(15);
    }

    [Fact]
    public void AwardPoints_ZeroPoints_ThrowsArgumentException()
    {
        // Arrange
        var member = new Member(
            Guid.NewGuid(),
            "Alex",
            MemberRole.Child);

        // Act
        var act = () => member.AwardPoints(0);

        // Assert
        act.Should()
            .Throw<ArgumentException>()
            .WithMessage("*Points must be greater than zero*");
    }

    [Fact]
    public void AwardPoints_NegativePoints_ThrowsArgumentException()
    {
        // Arrange
        var member = new Member(
            Guid.NewGuid(),
            "Alex",
            MemberRole.Child);

        // Act
        var act = () => member.AwardPoints(-1);

        // Assert
        act.Should()
            .Throw<ArgumentException>()
            .WithMessage("*Points must be greater than zero*");
    }
}