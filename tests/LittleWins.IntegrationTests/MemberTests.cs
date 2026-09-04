using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using LittleWins.Application.UseCases.Members.AddMember;
using LittleWins.Domain.Enums;

namespace LittleWins.IntegrationTests;

[Collection("Integration tests")]
public sealed class MemberTests : IntegrationTest
{
    private const string FamilyName = "The Smith Family";
    private const string MemberName = "Alex";
    private const string ParentName = "Jordan";
    private const string WhitespaceName = "   ";

    private const char TestCharacter = 'A';

    private const int MaximumNameLength = 100;
    private const int InvalidNameLength = MaximumNameLength + 1;
    private const int InvalidRoleValue = 999;

    public MemberTests(IntegrationTestFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task AddMemberReturnsCreatedWithMemberDetails()
    {
        // Arrange
        var familyId =
            await IntegrationTestHelpers.CreateFamilyAsync(
                Client,
                FamilyName);

        var command = new AddMemberCommand(
            familyId,
            MemberName,
            MemberRole.Child);

        // Act
        var response = await Client.PostAsJsonAsync(
            $"/api/families/{familyId}/members",
            command);

        var result = await response.Content
            .ReadFromJsonAsync<AddMemberResult>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        result.Should().NotBeNull();
        result!.MemberId.Should().NotBeEmpty();
        result.FamilyId.Should().Be(familyId);
        result.Name.Should().Be(MemberName);

        response.Headers.Location.Should().Be(
            $"/api/families/{familyId}/members/{result.MemberId}");
    }

    [Fact]
    public async Task AddParentReturnsCreatedWithMemberDetails()
    {
        // Arrange
        var familyId =
            await IntegrationTestHelpers.CreateFamilyAsync(
                Client,
                FamilyName);

        var command = new AddMemberCommand(
            familyId,
            ParentName,
            MemberRole.Parent);

        // Act
        var response = await Client.PostAsJsonAsync(
            $"/api/families/{familyId}/members",
            command);

        var result = await response.Content
            .ReadFromJsonAsync<AddMemberResult>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        result.Should().NotBeNull();
        result!.MemberId.Should().NotBeEmpty();
        result.FamilyId.Should().Be(familyId);
        result.Name.Should().Be(ParentName);
    }

    [Fact]
    public async Task AddMemberWithMismatchedFamilyIdReturnsBadRequest()
    {
        // Arrange
        var familyId =
            await IntegrationTestHelpers.CreateFamilyAsync(
                Client,
                FamilyName);

        var command = new AddMemberCommand(
            Guid.NewGuid(),
            MemberName,
            MemberRole.Child);

        // Act
        var response = await Client.PostAsJsonAsync(
            $"/api/families/{familyId}/members",
            command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AddMemberWithEmptyNameReturnsBadRequest()
    {
        // Arrange
        var familyId =
            await IntegrationTestHelpers.CreateFamilyAsync(
                Client,
                FamilyName);

        var command = new AddMemberCommand(
            familyId,
            string.Empty,
            MemberRole.Child);

        // Act
        var response = await Client.PostAsJsonAsync(
            $"/api/families/{familyId}/members",
            command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AddMemberWithWhitespaceNameReturnsBadRequest()
    {
        // Arrange
        var familyId =
            await IntegrationTestHelpers.CreateFamilyAsync(
                Client,
                FamilyName);

        var command = new AddMemberCommand(
            familyId,
            WhitespaceName,
            MemberRole.Child);

        // Act
        var response = await Client.PostAsJsonAsync(
            $"/api/families/{familyId}/members",
            command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AddMemberWithMaximumLengthNameReturnsCreated()
    {
        // Arrange
        var familyId =
            await IntegrationTestHelpers.CreateFamilyAsync(
                Client,
                FamilyName);

        var memberName = new string(
            TestCharacter,
            MaximumNameLength);

        var command = new AddMemberCommand(
            familyId,
            memberName,
            MemberRole.Child);

        // Act
        var response = await Client.PostAsJsonAsync(
            $"/api/families/{familyId}/members",
            command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task AddMemberWithNameExceedingMaximumLengthReturnsBadRequest()
    {
        // Arrange
        var familyId =
            await IntegrationTestHelpers.CreateFamilyAsync(
                Client,
                FamilyName);

        var memberName = new string(
            TestCharacter,
            InvalidNameLength);

        var command = new AddMemberCommand(
            familyId,
            memberName,
            MemberRole.Child);

        // Act
        var response = await Client.PostAsJsonAsync(
            $"/api/families/{familyId}/members",
            command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AddMemberWithInvalidRoleReturnsBadRequest()
    {
        // Arrange
        var familyId =
            await IntegrationTestHelpers.CreateFamilyAsync(
                Client,
                FamilyName);

        var invalidRole = (MemberRole)InvalidRoleValue;

        var command = new AddMemberCommand(
            familyId,
            MemberName,
            invalidRole);

        // Act
        var response = await Client.PostAsJsonAsync(
            $"/api/families/{familyId}/members",
            command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AddMemberWithNonExistentFamilyReturnsInternalServerError()
    {
        // Arrange
        var familyId = Guid.NewGuid();

        var command = new AddMemberCommand(
            familyId,
            MemberName,
            MemberRole.Child);

        // Act
        var response = await Client.PostAsJsonAsync(
            $"/api/families/{familyId}/members",
            command);

        // Assert
        response.StatusCode.Should().Be(
            HttpStatusCode.InternalServerError);
    }
}