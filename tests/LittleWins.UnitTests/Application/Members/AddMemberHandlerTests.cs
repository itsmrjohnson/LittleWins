using FluentAssertions;
using LittleWins.Application.UseCases.Members.AddMember;
using LittleWins.Domain.Entities;
using LittleWins.Domain.Enums;
using LittleWins.UnitTests.Fakes;

namespace LittleWins.UnitTests.Application.Members;

public class AddMemberHandlerTests
{
    [Fact]
    public async Task HandleAsync_ValidCommand_AddsMemberToFamily()
    {
        // Arrange
        var family = CreateFamily();

        var familyRepository = new FakeFamilyRepository(family);
        var memberRepository = new FakeMemberRepository();
        var unitOfWork = new FakeUnitOfWork();

        var handler = new AddMemberHandler(
            familyRepository,
            memberRepository,
            unitOfWork);

        var command = new AddMemberCommand(
            family.Id,
            "Alex",
            MemberRole.Child);

        // Act
        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        // Assert
        result.Name.Should().Be("Alex");
        result.FamilyId.Should().Be(family.Id);
        result.MemberId.Should().NotBe(Guid.Empty);

        memberRepository.AddedMember.Should().NotBeNull();
        memberRepository.AddedMember!.Name.Should().Be("Alex");
        memberRepository.AddedMember.FamilyId.Should().Be(family.Id);
        memberRepository.AddedMember.Role.Should().Be(MemberRole.Child);

        family.Members.Should().ContainSingle();

        unitOfWork.SaveChangesCallCount.Should().Be(1);
    }

    [Fact]
    public async Task HandleAsync_FamilyDoesNotExist_ThrowsInvalidOperationException()
    {
        // Arrange
        var familyRepository = new FakeFamilyRepository();
        var memberRepository = new FakeMemberRepository();
        var unitOfWork = new FakeUnitOfWork();

        var handler = new AddMemberHandler(
            familyRepository,
            memberRepository,
            unitOfWork);

        var command = new AddMemberCommand(
            Guid.NewGuid(),
            "Alex",
            MemberRole.Child);

        // Act
        var act = () => handler.HandleAsync(
            command,
            CancellationToken.None);

        // Assert
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Family was not found.");

        memberRepository.AddedMember.Should().BeNull();
        unitOfWork.SaveChangesCallCount.Should().Be(0);
    }

    private static Family CreateFamily()
    {
        return new Family("The Smith Family");
    }
}