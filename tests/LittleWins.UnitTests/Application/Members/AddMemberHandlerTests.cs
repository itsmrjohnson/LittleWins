using FluentAssertions;
using LittleWins.Application.UseCases.Members.AddMember;
using LittleWins.Domain.Entities;
using LittleWins.Domain.Enums;
using LittleWins.UnitTests.Fakes;

namespace LittleWins.UnitTests.Application.Members;

public class AddMemberHandlerTests
{
    private const string FamilyName = "The Smith Family";
    private const string MemberName = "Alex";
    private const string MissingFamilyMessage = "Family was not found.";
    private const int MaximumNameLength = 100;

    [Fact]
    public async Task HandleAsyncValidCommandAddsMemberToFamily()
    {
        // Arrange
        var family = CreateFamily();

        var familyRepository = new FakeFamilyRepository(family);
        var memberRepository = new FakeMemberRepository();
        var unitOfWork = new FakeUnitOfWork();
        var validator = new AddMemberCommandValidator();

        var handler = new AddMemberHandler(
            familyRepository,
            memberRepository,
            unitOfWork,
            validator);

        var command = new AddMemberCommand(
            family.Id,
            MemberName,
            MemberRole.Child);

        // Act
        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        // Assert
        result.Name.Should().Be(MemberName);
        result.FamilyId.Should().Be(family.Id);
        result.MemberId.Should().NotBe(Guid.Empty);

        memberRepository.AddedMember.Should().NotBeNull();
        memberRepository.AddedMember!.Name.Should().Be(MemberName);
        memberRepository.AddedMember.FamilyId.Should().Be(family.Id);
        memberRepository.AddedMember.Role.Should().Be(MemberRole.Child);

        family.Members.Should().ContainSingle();

        unitOfWork.SaveChangesCallCount.Should().Be(1);
    }

    [Fact]
    public async Task HandleAsyncFamilyDoesNotExistThrowsInvalidOperationException()
    {
        // Arrange
        var familyRepository = new FakeFamilyRepository();
        var memberRepository = new FakeMemberRepository();
        var unitOfWork = new FakeUnitOfWork();
        var validator = new AddMemberCommandValidator();

        var handler = new AddMemberHandler(
            familyRepository,
            memberRepository,
            unitOfWork,
            validator);

        var command = new AddMemberCommand(
            Guid.NewGuid(),
            MemberName,
            MemberRole.Child);

        // Act
        var act = () => handler.HandleAsync(
            command,
            CancellationToken.None);

        // Assert
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage(MissingFamilyMessage);

        memberRepository.AddedMember.Should().BeNull();
        unitOfWork.SaveChangesCallCount.Should().Be(0);
    }

    private static Family CreateFamily()
    {
        return new Family(FamilyName);
    }
}