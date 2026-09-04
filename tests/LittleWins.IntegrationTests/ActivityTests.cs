using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using LittleWins.Application.UseCases.Activities.CreateActivity;
using LittleWins.Domain.Enums;

namespace LittleWins.IntegrationTests;

[Collection("Integration tests")]
public sealed class ActivityTests : IntegrationTest
{
    private const string FamilyName = "The Smith Family";
    private const string MemberName = "Alex";
    private const string OtherFamilyName = "The Jones Family";

    private const string ActivityTitle = "Brush your teeth";
    private const string ActivityDescription =
        "Brush your teeth before bed.";
    private const string ActivityCategory = "Health";
    private const string WhitespaceTitle = "   ";

    private const char TestCharacter = 'A';

    private const int ActivityPoints = 10;
    private const int ZeroPoints = 0;
    private const int NegativePoints = -1;

    private const int MaximumTitleLength = 200;
    private const int InvalidTitleLength = MaximumTitleLength + 1;

    private const int MaximumDescriptionLength = 1000;
    private const int InvalidDescriptionLength =
        MaximumDescriptionLength + 1;

    private const int MaximumCategoryLength = 100;
    private const int InvalidCategoryLength =
        MaximumCategoryLength + 1;

    public ActivityTests(IntegrationTestFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task CreateActivityReturnsCreatedWithActivityDetails()
    {
        // Arrange
        var familyId =
            await IntegrationTestHelpers.CreateFamilyAsync(
                Client,
                FamilyName);

        var memberId =
            await IntegrationTestHelpers.AddMemberAsync(
                Client,
                familyId,
                MemberName,
                MemberRole.Child);

        var command = new CreateActivityCommand(
            familyId,
            ActivityTitle,
            ActivityDescription,
            ActivityCategory,
            memberId,
            ActivityPoints,
            null,
            true);

        // Act
        var response = await Client.PostAsJsonAsync(
            "/api/activities",
            command);

        var result = await response.Content
            .ReadFromJsonAsync<CreateActivityResult>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        result.Should().NotBeNull();
        result!.ActivityId.Should().NotBeEmpty();
        result.FamilyId.Should().Be(familyId);
        result.Title.Should().Be(ActivityTitle);
        result.AssignedToMemberId.Should().Be(memberId);
        result.Points.Should().Be(ActivityPoints);

        response.Headers.Location.Should().Be(
            $"/api/activities/{result.ActivityId}");
    }

    [Fact]
    public async Task CreateActivityWithEmptyTitleReturnsBadRequest()
    {
        // Arrange
        var familyId =
            await IntegrationTestHelpers.CreateFamilyAsync(
                Client,
                FamilyName);

        var memberId =
            await IntegrationTestHelpers.AddMemberAsync(
                Client,
                familyId,
                MemberName,
                MemberRole.Child);

        var command = new CreateActivityCommand(
            familyId,
            string.Empty,
            ActivityDescription,
            ActivityCategory,
            memberId,
            ActivityPoints,
            null,
            true);

        // Act
        var response = await Client.PostAsJsonAsync(
            "/api/activities",
            command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateActivityWithWhitespaceTitleReturnsBadRequest()
    {
        // Arrange
        var familyId =
            await IntegrationTestHelpers.CreateFamilyAsync(
                Client,
                FamilyName);

        var memberId =
            await IntegrationTestHelpers.AddMemberAsync(
                Client,
                familyId,
                MemberName,
                MemberRole.Child);

        var command = new CreateActivityCommand(
            familyId,
            WhitespaceTitle,
            ActivityDescription,
            ActivityCategory,
            memberId,
            ActivityPoints,
            null,
            true);

        // Act
        var response = await Client.PostAsJsonAsync(
            "/api/activities",
            command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateActivityWithMaximumLengthTitleReturnsCreated()
    {
        // Arrange
        var familyId =
            await IntegrationTestHelpers.CreateFamilyAsync(
                Client,
                FamilyName);

        var memberId =
            await IntegrationTestHelpers.AddMemberAsync(
                Client,
                familyId,
                MemberName,
                MemberRole.Child);

        var title = new string(
            TestCharacter,
            MaximumTitleLength);

        var command = new CreateActivityCommand(
            familyId,
            title,
            ActivityDescription,
            ActivityCategory,
            memberId,
            ActivityPoints,
            null,
            true);

        // Act
        var response = await Client.PostAsJsonAsync(
            "/api/activities",
            command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task CreateActivityWithTitleExceedingMaximumLengthReturnsBadRequest()
    {
        // Arrange
        var familyId =
            await IntegrationTestHelpers.CreateFamilyAsync(
                Client,
                FamilyName);

        var memberId =
            await IntegrationTestHelpers.AddMemberAsync(
                Client,
                familyId,
                MemberName,
                MemberRole.Child);

        var title = new string(
            TestCharacter,
            InvalidTitleLength);

        var command = new CreateActivityCommand(
            familyId,
            title,
            ActivityDescription,
            ActivityCategory,
            memberId,
            ActivityPoints,
            null,
            true);

        // Act
        var response = await Client.PostAsJsonAsync(
            "/api/activities",
            command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateActivityWithMaximumLengthDescriptionReturnsCreated()
    {
        // Arrange
        var familyId =
            await IntegrationTestHelpers.CreateFamilyAsync(
                Client,
                FamilyName);

        var memberId =
            await IntegrationTestHelpers.AddMemberAsync(
                Client,
                familyId,
                MemberName,
                MemberRole.Child);

        var description = new string(
            TestCharacter,
            MaximumDescriptionLength);

        var command = new CreateActivityCommand(
            familyId,
            ActivityTitle,
            description,
            ActivityCategory,
            memberId,
            ActivityPoints,
            null,
            true);

        // Act
        var response = await Client.PostAsJsonAsync(
            "/api/activities",
            command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task CreateActivityWithDescriptionExceedingMaximumLengthReturnsBadRequest()
    {
        // Arrange
        var familyId =
            await IntegrationTestHelpers.CreateFamilyAsync(
                Client,
                FamilyName);

        var memberId =
            await IntegrationTestHelpers.AddMemberAsync(
                Client,
                familyId,
                MemberName,
                MemberRole.Child);

        var description = new string(
            TestCharacter,
            InvalidDescriptionLength);

        var command = new CreateActivityCommand(
            familyId,
            ActivityTitle,
            description,
            ActivityCategory,
            memberId,
            ActivityPoints,
            null,
            true);

        // Act
        var response = await Client.PostAsJsonAsync(
            "/api/activities",
            command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateActivityWithEmptyCategoryReturnsBadRequest()
    {
        // Arrange
        var familyId =
            await IntegrationTestHelpers.CreateFamilyAsync(
                Client,
                FamilyName);

        var memberId =
            await IntegrationTestHelpers.AddMemberAsync(
                Client,
                familyId,
                MemberName,
                MemberRole.Child);

        var command = new CreateActivityCommand(
            familyId,
            ActivityTitle,
            ActivityDescription,
            string.Empty,
            memberId,
            ActivityPoints,
            null,
            true);

        // Act
        var response = await Client.PostAsJsonAsync(
            "/api/activities",
            command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateActivityWithMaximumLengthCategoryReturnsCreated()
    {
        // Arrange
        var familyId =
            await IntegrationTestHelpers.CreateFamilyAsync(
                Client,
                FamilyName);

        var memberId =
            await IntegrationTestHelpers.AddMemberAsync(
                Client,
                familyId,
                MemberName,
                MemberRole.Child);

        var category = new string(
            TestCharacter,
            MaximumCategoryLength);

        var command = new CreateActivityCommand(
            familyId,
            ActivityTitle,
            ActivityDescription,
            category,
            memberId,
            ActivityPoints,
            null,
            true);

        // Act
        var response = await Client.PostAsJsonAsync(
            "/api/activities",
            command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task CreateActivityWithCategoryExceedingMaximumLengthReturnsBadRequest()
    {
        // Arrange
        var familyId =
            await IntegrationTestHelpers.CreateFamilyAsync(
                Client,
                FamilyName);

        var memberId =
            await IntegrationTestHelpers.AddMemberAsync(
                Client,
                familyId,
                MemberName,
                MemberRole.Child);

        var category = new string(
            TestCharacter,
            InvalidCategoryLength);

        var command = new CreateActivityCommand(
            familyId,
            ActivityTitle,
            ActivityDescription,
            category,
            memberId,
            ActivityPoints,
            null,
            true);

        // Act
        var response = await Client.PostAsJsonAsync(
            "/api/activities",
            command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateActivityWithZeroPointsReturnsBadRequest()
    {
        // Arrange
        var familyId =
            await IntegrationTestHelpers.CreateFamilyAsync(
                Client,
                FamilyName);

        var memberId =
            await IntegrationTestHelpers.AddMemberAsync(
                Client,
                familyId,
                MemberName,
                MemberRole.Child);

        var command = new CreateActivityCommand(
            familyId,
            ActivityTitle,
            ActivityDescription,
            ActivityCategory,
            memberId,
            ZeroPoints,
            null,
            true);

        // Act
        var response = await Client.PostAsJsonAsync(
            "/api/activities",
            command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateActivityWithNegativePointsReturnsBadRequest()
    {
        // Arrange
        var familyId =
            await IntegrationTestHelpers.CreateFamilyAsync(
                Client,
                FamilyName);

        var memberId =
            await IntegrationTestHelpers.AddMemberAsync(
                Client,
                familyId,
                MemberName,
                MemberRole.Child);

        var command = new CreateActivityCommand(
            familyId,
            ActivityTitle,
            ActivityDescription,
            ActivityCategory,
            memberId,
            NegativePoints,
            null,
            true);

        // Act
        var response = await Client.PostAsJsonAsync(
            "/api/activities",
            command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateActivityWithNonExistentFamilyReturnsInternalServerError()
    {
        // Arrange
        var command = new CreateActivityCommand(
            Guid.NewGuid(),
            ActivityTitle,
            ActivityDescription,
            ActivityCategory,
            Guid.NewGuid(),
            ActivityPoints,
            null,
            true);

        // Act
        var response = await Client.PostAsJsonAsync(
            "/api/activities",
            command);

        // Assert
        response.StatusCode.Should().Be(
            HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task CreateActivityWithNonExistentMemberReturnsInternalServerError()
    {
        // Arrange
        var familyId =
            await IntegrationTestHelpers.CreateFamilyAsync(
                Client,
                FamilyName);

        var command = new CreateActivityCommand(
            familyId,
            ActivityTitle,
            ActivityDescription,
            ActivityCategory,
            Guid.NewGuid(),
            ActivityPoints,
            null,
            true);

        // Act
        var response = await Client.PostAsJsonAsync(
            "/api/activities",
            command);

        // Assert
        response.StatusCode.Should().Be(
            HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task CreateActivityWithMemberFromDifferentFamilyReturnsInternalServerError()
    {
        // Arrange
        var familyId =
            await IntegrationTestHelpers.CreateFamilyAsync(
                Client,
                FamilyName);

        var otherFamilyId =
            await IntegrationTestHelpers.CreateFamilyAsync(
                Client,
                OtherFamilyName);

        var memberId =
            await IntegrationTestHelpers.AddMemberAsync(
                Client,
                otherFamilyId,
                MemberName,
                MemberRole.Child);

        var command = new CreateActivityCommand(
            familyId,
            ActivityTitle,
            ActivityDescription,
            ActivityCategory,
            memberId,
            ActivityPoints,
            null,
            true);

        // Act
        var response = await Client.PostAsJsonAsync(
            "/api/activities",
            command);

        // Assert
        response.StatusCode.Should().Be(
            HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task CreateActivityWithFutureDueDateReturnsCreated()
    {
        // Arrange
        var familyId =
            await IntegrationTestHelpers.CreateFamilyAsync(
                Client,
                FamilyName);

        var memberId =
            await IntegrationTestHelpers.AddMemberAsync(
                Client,
                familyId,
                MemberName,
                MemberRole.Child);

        var dueDate = DateTime.UtcNow.AddDays(1);

        var command = new CreateActivityCommand(
            familyId,
            ActivityTitle,
            ActivityDescription,
            ActivityCategory,
            memberId,
            ActivityPoints,
            dueDate,
            true);

        // Act
        var response = await Client.PostAsJsonAsync(
            "/api/activities",
            command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task CreateActivityWithPastDueDateReturnsBadRequest()
    {
        // Arrange
        var familyId =
            await IntegrationTestHelpers.CreateFamilyAsync(
                Client,
                FamilyName);

        var memberId =
            await IntegrationTestHelpers.AddMemberAsync(
                Client,
                familyId,
                MemberName,
                MemberRole.Child);

        var dueDate = DateTime.UtcNow.AddDays(-1);

        var command = new CreateActivityCommand(
            familyId,
            ActivityTitle,
            ActivityDescription,
            ActivityCategory,
            memberId,
            ActivityPoints,
            dueDate,
            true);

        // Act
        var response = await Client.PostAsJsonAsync(
            "/api/activities",
            command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateActivityWithoutApprovalReturnsCreated()
    {
        // Arrange
        var familyId =
            await IntegrationTestHelpers.CreateFamilyAsync(
                Client,
                FamilyName);

        var memberId =
            await IntegrationTestHelpers.AddMemberAsync(
                Client,
                familyId,
                MemberName,
                MemberRole.Child);

        var command = new CreateActivityCommand(
            familyId,
            ActivityTitle,
            ActivityDescription,
            ActivityCategory,
            memberId,
            ActivityPoints,
            null,
            false);

        // Act
        var response = await Client.PostAsJsonAsync(
            "/api/activities",
            command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}