using FluentAssertions;
using FluentValidation;
using LittleWins.Application.UseCases.Families.CreateFamily;
using LittleWins.UnitTests.Fakes;

namespace LittleWins.UnitTests.Application.Families;

public class CreateFamilyHandlerTests
{
    private const string FamilyName = "The Smith Family";
    private const string EmptyName = "";
    private const string WhitespaceName = "   ";
    private const char NameCharacter = 'A';
    private const int MaximumNameLength = 100;
    private const int InvalidNameLength = MaximumNameLength + 1;

    [Fact]
    public async Task HandleAsyncValidCommandCreatesFamily()
    {
        // Arrange
        var familyRepository = new FakeFamilyRepository();
        var unitOfWork = new FakeUnitOfWork();
        var validator = new CreateFamilyCommandValidator();

        var handler = new CreateFamilyHandler(
            familyRepository,
            unitOfWork,
            validator);

        var command = new CreateFamilyCommand(FamilyName);

        // Act
        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        // Assert
        result.Name.Should().Be(FamilyName);
        result.FamilyId.Should().NotBe(Guid.Empty);

        familyRepository.AddedFamily.Should().NotBeNull();
        familyRepository.AddedFamily!.Name.Should().Be(FamilyName);

        unitOfWork.SaveChangesCallCount.Should().Be(1);
    }

    [Fact]
    public async Task HandleAsyncMaximumLengthNameCreatesFamily()
    {
        // Arrange
        var familyRepository = new FakeFamilyRepository();
        var unitOfWork = new FakeUnitOfWork();
        var validator = new CreateFamilyCommandValidator();

        var handler = new CreateFamilyHandler(
            familyRepository,
            unitOfWork,
            validator);

        var familyName = new string(
            NameCharacter,
            MaximumNameLength);

        var command = new CreateFamilyCommand(familyName);

        // Act
        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        // Assert
        result.Name.Should().HaveLength(MaximumNameLength);
        result.FamilyId.Should().NotBe(Guid.Empty);

        familyRepository.AddedFamily.Should().NotBeNull();
        unitOfWork.SaveChangesCallCount.Should().Be(1);
    }

    [Fact]
    public async Task HandleAsyncInvalidCommandThrowsValidationException()
    {
        // Arrange
        var familyRepository = new FakeFamilyRepository();
        var unitOfWork = new FakeUnitOfWork();
        var validator = new CreateFamilyCommandValidator();

        var handler = new CreateFamilyHandler(
            familyRepository,
            unitOfWork,
            validator);

        var command = new CreateFamilyCommand(EmptyName);

        // Act
        Func<Task> act = () => handler.HandleAsync(
            command,
            CancellationToken.None);

        // Assert
        await act.Should()
            .ThrowAsync<ValidationException>();

        familyRepository.AddedFamily.Should().BeNull();
        unitOfWork.SaveChangesCallCount.Should().Be(0);
    }

    [Fact]
    public async Task HandleAsyncWhitespaceNameThrowsValidationException()
    {
        // Arrange
        var familyRepository = new FakeFamilyRepository();
        var unitOfWork = new FakeUnitOfWork();
        var validator = new CreateFamilyCommandValidator();

        var handler = new CreateFamilyHandler(
            familyRepository,
            unitOfWork,
            validator);

        var command = new CreateFamilyCommand(WhitespaceName);

        // Act
        Func<Task> act = () => handler.HandleAsync(
            command,
            CancellationToken.None);

        // Assert
        await act.Should()
            .ThrowAsync<ValidationException>();

        familyRepository.AddedFamily.Should().BeNull();
        unitOfWork.SaveChangesCallCount.Should().Be(0);
    }

    [Fact]
    public async Task HandleAsyncNameExceedsMaximumLengthThrowsValidationException()
    {
        // Arrange
        var familyRepository = new FakeFamilyRepository();
        var unitOfWork = new FakeUnitOfWork();
        var validator = new CreateFamilyCommandValidator();

        var handler = new CreateFamilyHandler(
            familyRepository,
            unitOfWork,
            validator);

        var familyName = new string(
            NameCharacter,
            InvalidNameLength);

        var command = new CreateFamilyCommand(familyName);

        // Act
        Func<Task> act = () => handler.HandleAsync(
            command,
            CancellationToken.None);

        // Assert
        await act.Should()
            .ThrowAsync<ValidationException>();

        familyRepository.AddedFamily.Should().BeNull();
        unitOfWork.SaveChangesCallCount.Should().Be(0);
    }
}