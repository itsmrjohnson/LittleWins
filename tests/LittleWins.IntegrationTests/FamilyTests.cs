using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using LittleWins.Application.UseCases.Families.CreateFamily;

namespace LittleWins.IntegrationTests;

[Collection("Integration tests")]
public sealed class FamilyTests : IntegrationTest
{
    private const string FamilyName = "The Smith Family";
    private const string EmptyName = "";
    private const string WhitespaceName = "   ";

    private const char TestCharacter = 'A';

    private const int MaximumNameLength = 100;
    private const int InvalidNameLength = MaximumNameLength + 1;

    public FamilyTests(IntegrationTestFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task CreateFamilyReturnsCreatedWithFamilyDetails()
    {
        // Arrange
        var command = new CreateFamilyCommand(FamilyName);

        // Act
        var response = await Client.PostAsJsonAsync(
            "/api/families",
            command);

        var result = await response.Content
            .ReadFromJsonAsync<CreateFamilyResult>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        result.Should().NotBeNull();
        result!.FamilyId.Should().NotBeEmpty();
        result.Name.Should().Be(FamilyName);

        response.Headers.Location.Should().Be(
            $"/api/families/{result.FamilyId}");
    }

    [Fact]
    public async Task CreateFamilyWithEmptyNameReturnsBadRequest()
    {
        // Arrange
        var command = new CreateFamilyCommand(EmptyName);

        // Act
        var response = await Client.PostAsJsonAsync(
            "/api/families",
            command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateFamilyWithWhitespaceNameReturnsBadRequest()
    {
        // Arrange
        var command = new CreateFamilyCommand(WhitespaceName);

        // Act
        var response = await Client.PostAsJsonAsync(
            "/api/families",
            command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateFamilyWithMaximumLengthNameReturnsCreated()
    {
        // Arrange
        var familyName = new string(
            TestCharacter,
            MaximumNameLength);

        var command = new CreateFamilyCommand(familyName);

        // Act
        var response = await Client.PostAsJsonAsync(
            "/api/families",
            command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task CreateFamilyWithNameExceedingMaximumLengthReturnsBadRequest()
    {
        // Arrange
        var familyName = new string(
            TestCharacter,
            InvalidNameLength);

        var command = new CreateFamilyCommand(familyName);

        // Act
        var response = await Client.PostAsJsonAsync(
            "/api/families",
            command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}