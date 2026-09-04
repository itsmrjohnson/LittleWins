using FluentAssertions;
using LittleWins.Application.UseCases.Activities.CreateActivity;
using LittleWins.Domain.Entities;
using LittleWins.Domain.Enums;
using LittleWins.UnitTests.Fakes;

namespace LittleWins.UnitTests.Application.Activities;

public class CreateActivityHandlerTests
{
    [Fact]
    public async Task HandleAsync_ValidCommand_CreatesActivity()
    {
        // Arrange
        var family = CreateFamily();
        var member = CreateChild(family);

        var familyRepository = new FakeFamilyRepository(family);
        var memberRepository = new FakeMemberRepository(member);
        var activityRepository = new FakeActivityRepository();
        var unitOfWork = new FakeUnitOfWork();

        var validator = new CreateActivityCommandValidator();

        var handler = new CreateActivityHandler(
            familyRepository,
            memberRepository,
            activityRepository,
            unitOfWork,
            validator);

        var command = new CreateActivityCommand(
            family.Id,
            "Clean bedroom",
            "Tidy the bedroom",
            "Chore",
            member.Id,
            10,
            null,
            true);

        // Act
        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        // Assert
        result.ActivityId.Should().NotBe(Guid.Empty);
        result.FamilyId.Should().Be(family.Id);
        result.Title.Should().Be("Clean bedroom");
        result.AssignedToMemberId.Should().Be(member.Id);
        result.Points.Should().Be(10);

        activityRepository.AddedActivity.Should().NotBeNull();
        activityRepository.AddedActivity!.FamilyId.Should().Be(family.Id);
        activityRepository.AddedActivity.AssignedToMemberId.Should().Be(member.Id);
        activityRepository.AddedActivity.Points.Should().Be(10);

        family.Activities.Should().ContainSingle();

        unitOfWork.SaveChangesCallCount.Should().Be(1);
    }

    [Fact]
    public async Task HandleAsync_FamilyDoesNotExist_ThrowsInvalidOperationException()
    {
        // Arrange
        var familyRepository = new FakeFamilyRepository();
        var memberRepository = new FakeMemberRepository();
        var activityRepository = new FakeActivityRepository();
        var unitOfWork = new FakeUnitOfWork();

        var validator = new CreateActivityCommandValidator();

        var handler = new CreateActivityHandler(
            familyRepository,
            memberRepository,
            activityRepository,
            unitOfWork,
            validator);

        var command = new CreateActivityCommand(
            Guid.NewGuid(),
            "Clean bedroom",
            null,
            "Chore",
            Guid.NewGuid(),
            10,
            null,
            true);

        // Act
        var act = () => handler.HandleAsync(
            command,
            CancellationToken.None);

        // Assert
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Family was not found.");

        activityRepository.AddedActivity.Should().BeNull();
        unitOfWork.SaveChangesCallCount.Should().Be(0);
    }

    [Fact]
    public async Task HandleAsync_AssignedMemberDoesNotExist_ThrowsInvalidOperationException()
    {
        // Arrange
        var family = CreateFamily();

        var familyRepository = new FakeFamilyRepository(family);
        var memberRepository = new FakeMemberRepository();
        var activityRepository = new FakeActivityRepository();
        var unitOfWork = new FakeUnitOfWork();

        var validator = new CreateActivityCommandValidator();

        var handler = new CreateActivityHandler(
            familyRepository,
            memberRepository,
            activityRepository,
            unitOfWork,
            validator);

        var command = new CreateActivityCommand(
            family.Id,
            "Clean bedroom",
            null,
            "Chore",
            Guid.NewGuid(),
            10,
            null,
            true);

        // Act
        var act = () => handler.HandleAsync(
            command,
            CancellationToken.None);

        // Assert
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Assigned member was not found.");

        activityRepository.AddedActivity.Should().BeNull();
        unitOfWork.SaveChangesCallCount.Should().Be(0);
    }

    [Fact]
    public async Task HandleAsync_AssignedMemberBelongsToDifferentFamily_ThrowsInvalidOperationException()
    {
        // Arrange
        var family = CreateFamily();
        var differentFamily = new Family("The Jones Family");
        var member = CreateChild(differentFamily);

        var familyRepository = new FakeFamilyRepository(family);
        var memberRepository = new FakeMemberRepository(member);
        var activityRepository = new FakeActivityRepository();
        var unitOfWork = new FakeUnitOfWork();

        var validator = new CreateActivityCommandValidator();

        var handler = new CreateActivityHandler(
            familyRepository,
            memberRepository,
            activityRepository,
            unitOfWork,
            validator);

        var command = new CreateActivityCommand(
            family.Id,
            "Clean bedroom",
            null,
            "Chore",
            member.Id,
            10,
            null,
            true);

        // Act
        var act = () => handler.HandleAsync(
            command,
            CancellationToken.None);

        // Assert
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Assigned member does not belong to the family.");

        activityRepository.AddedActivity.Should().BeNull();
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
}