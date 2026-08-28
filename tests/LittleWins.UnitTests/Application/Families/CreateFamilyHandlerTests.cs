using FluentAssertions;
using LittleWins.Application.UseCases.Families.CreateFamily;
using LittleWins.UnitTests.Fakes;

namespace LittleWins.UnitTests.Application.Families;

public class CreateFamilyHandlerTests
{
    [Fact]
    public async Task HandleAsync_ValidCommand_CreatesFamily()
    {
        // Arrange
        const string familyName = "The Smith Family";

        var familyRepository = new FakeFamilyRepository();
        var unitOfWork = new FakeUnitOfWork();

        var handler = new CreateFamilyHandler(
            familyRepository,
            unitOfWork);

        var command = new CreateFamilyCommand(familyName);

        // Act
        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        // Assert
        result.Name.Should().Be(familyName);
        result.FamilyId.Should().NotBe(Guid.Empty);

        familyRepository.AddedFamily.Should().NotBeNull();
        familyRepository.AddedFamily!.Name.Should().Be(familyName);

        unitOfWork.SaveChangesCallCount.Should().Be(1);
    }

    [Fact]
    public async Task HandleAsync_ValidCommand_PersistsFamily()
    {
        // Arrange
        var familyRepository = new FakeFamilyRepository();
        var unitOfWork = new FakeUnitOfWork();

        var handler = new CreateFamilyHandler(
            familyRepository,
            unitOfWork);

        var command = new CreateFamilyCommand(
            "The Smith Family");

        // Act
        await handler.HandleAsync(
            command,
            CancellationToken.None);

        // Assert
        familyRepository.AddedFamily.Should().NotBeNull();
        unitOfWork.SaveChangesCallCount.Should().Be(1);
    }
}