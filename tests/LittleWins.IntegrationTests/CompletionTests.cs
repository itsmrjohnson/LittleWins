using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using LittleWins.Application.UseCases.Completions.ApproveCompletion;
using LittleWins.Application.UseCases.Completions.CompleteActivity;
using LittleWins.Domain.Enums;

namespace LittleWins.IntegrationTests;

[Collection("Integration tests")]
public sealed class CompletionTests : IntegrationTest
{
    private const string FamilyName = "The Smith Family";
    private const string OtherFamilyName = "The Jones Family";

    private const string ChildMemberName = "Alex";
    private const string ParentMemberName = "Jordan";

    private const string ActivityTitle = "Brush your teeth";
    private const string ActivityDescription =
        "Brush your teeth before bed.";
    private const string ActivityCategory = "Health";

    private const int ActivityPoints = 10;

    public CompletionTests(IntegrationTestFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task CompleteActivityReturnsCreatedWithCompletionDetails()
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
                ChildMemberName,
                MemberRole.Child);

        var activityId =
            await IntegrationTestHelpers.CreateActivityAsync(
                Client,
                familyId,
                memberId,
                ActivityTitle,
                ActivityDescription,
                ActivityCategory,
                ActivityPoints,
                null,
                true);

        var command = new CompleteActivityCommand(
            activityId,
            memberId);

        // Act
        var response = await Client.PostAsJsonAsync(
            $"/api/activities/{activityId}/complete",
            command);

        var result = await response.Content
            .ReadFromJsonAsync<CompleteActivityResult>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        result.Should().NotBeNull();
        result!.CompletionId.Should().NotBeEmpty();
        result.ActivityId.Should().Be(activityId);
        result.MemberId.Should().Be(memberId);
        result.Status.Should().NotBeNullOrWhiteSpace();

        response.Headers.Location.Should().Be(
            $"/api/completions/{result.CompletionId}");
    }

    [Fact]
    public async Task CompleteActivityWithMismatchedActivityIdReturnsBadRequest()
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
                ChildMemberName,
                MemberRole.Child);

        var activityId =
            await IntegrationTestHelpers.CreateActivityAsync(
                Client,
                familyId,
                memberId,
                ActivityTitle,
                ActivityDescription,
                ActivityCategory,
                ActivityPoints,
                null,
                true);

        var command = new CompleteActivityCommand(
            Guid.NewGuid(),
            memberId);

        // Act
        var response = await Client.PostAsJsonAsync(
            $"/api/activities/{activityId}/complete",
            command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CompleteActivityWithEmptyMemberIdReturnsBadRequest()
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
                ChildMemberName,
                MemberRole.Child);

        var activityId =
            await IntegrationTestHelpers.CreateActivityAsync(
                Client,
                familyId,
                memberId,
                ActivityTitle,
                ActivityDescription,
                ActivityCategory,
                ActivityPoints,
                null,
                true);

        var command = new CompleteActivityCommand(
            activityId,
            Guid.Empty);

        // Act
        var response = await Client.PostAsJsonAsync(
            $"/api/activities/{activityId}/complete",
            command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CompleteActivityWithNonExistentActivityReturnsInternalServerError()
    {
        // Arrange
        var command = new CompleteActivityCommand(
            Guid.NewGuid(),
            Guid.NewGuid());

        // Act
        var response = await Client.PostAsJsonAsync(
            $"/api/activities/{command.ActivityId}/complete",
            command);

        // Assert
        response.StatusCode.Should().Be(
            HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task CompleteActivityWithNonExistentMemberReturnsInternalServerError()
    {
        // Arrange
        var familyId =
            await IntegrationTestHelpers.CreateFamilyAsync(
                Client,
                FamilyName);

        var assignedMemberId =
            await IntegrationTestHelpers.AddMemberAsync(
                Client,
                familyId,
                ChildMemberName,
                MemberRole.Child);

        var activityId =
            await IntegrationTestHelpers.CreateActivityAsync(
                Client,
                familyId,
                assignedMemberId,
                ActivityTitle,
                ActivityDescription,
                ActivityCategory,
                ActivityPoints,
                null,
                true);

        var command = new CompleteActivityCommand(
            activityId,
            Guid.NewGuid());

        // Act
        var response = await Client.PostAsJsonAsync(
            $"/api/activities/{activityId}/complete",
            command);

        // Assert
        response.StatusCode.Should().Be(
            HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task CompleteActivityWithMemberFromDifferentFamilyReturnsInternalServerError()
    {
        // Arrange
        var familyId =
            await IntegrationTestHelpers.CreateFamilyAsync(
                Client,
                FamilyName);

        var assignedMemberId =
            await IntegrationTestHelpers.AddMemberAsync(
                Client,
                familyId,
                ChildMemberName,
                MemberRole.Child);

        var activityId =
            await IntegrationTestHelpers.CreateActivityAsync(
                Client,
                familyId,
                assignedMemberId,
                ActivityTitle,
                ActivityDescription,
                ActivityCategory,
                ActivityPoints,
                null,
                true);

        var otherFamilyId =
            await IntegrationTestHelpers.CreateFamilyAsync(
                Client,
                OtherFamilyName);

        var otherMemberId =
            await IntegrationTestHelpers.AddMemberAsync(
                Client,
                otherFamilyId,
                ParentMemberName,
                MemberRole.Child);

        var command = new CompleteActivityCommand(
            activityId,
            otherMemberId);

        // Act
        var response = await Client.PostAsJsonAsync(
            $"/api/activities/{activityId}/complete",
            command);

        // Assert
        response.StatusCode.Should().Be(
            HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task CompleteActivityWithUnassignedMemberReturnsInternalServerError()
    {
        // Arrange
        var familyId =
            await IntegrationTestHelpers.CreateFamilyAsync(
                Client,
                FamilyName);

        var assignedMemberId =
            await IntegrationTestHelpers.AddMemberAsync(
                Client,
                familyId,
                ChildMemberName,
                MemberRole.Child);

        var otherMemberId =
            await IntegrationTestHelpers.AddMemberAsync(
                Client,
                familyId,
                ParentMemberName,
                MemberRole.Child);

        var activityId =
            await IntegrationTestHelpers.CreateActivityAsync(
                Client,
                familyId,
                assignedMemberId,
                ActivityTitle,
                ActivityDescription,
                ActivityCategory,
                ActivityPoints,
                null,
                true);

        var command = new CompleteActivityCommand(
            activityId,
            otherMemberId);

        // Act
        var response = await Client.PostAsJsonAsync(
            $"/api/activities/{activityId}/complete",
            command);

        // Assert
        response.StatusCode.Should().Be(
            HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task ApproveCompletionReturnsOkWithApprovalDetails()
    {
        // Arrange
        var familyId =
            await IntegrationTestHelpers.CreateFamilyAsync(
                Client,
                FamilyName);

        var childMemberId =
            await IntegrationTestHelpers.AddMemberAsync(
                Client,
                familyId,
                ChildMemberName,
                MemberRole.Child);

        var parentMemberId =
            await IntegrationTestHelpers.AddMemberAsync(
                Client,
                familyId,
                ParentMemberName,
                MemberRole.Parent);

        var activityId =
            await IntegrationTestHelpers.CreateActivityAsync(
                Client,
                familyId,
                childMemberId,
                ActivityTitle,
                ActivityDescription,
                ActivityCategory,
                ActivityPoints,
                null,
                true);

        var completionId =
            await IntegrationTestHelpers.CompleteActivityAsync(
                Client,
                activityId,
                childMemberId);

        var command = new ApproveCompletionCommand(
            completionId,
            parentMemberId);

        // Act
        var response = await Client.PostAsJsonAsync(
            $"/api/completions/{completionId}/approve",
            command);

        var result = await response.Content
            .ReadFromJsonAsync<ApproveCompletionResult>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        result.Should().NotBeNull();
        result!.CompletionId.Should().Be(completionId);
        result.MemberId.Should().Be(childMemberId);
        result.PointsAwarded.Should().Be(ActivityPoints);
    }

    [Fact]
    public async Task ApproveCompletionWithMismatchedCompletionIdReturnsBadRequest()
    {
        // Arrange
        var familyId =
            await IntegrationTestHelpers.CreateFamilyAsync(
                Client,
                FamilyName);

        var childMemberId =
            await IntegrationTestHelpers.AddMemberAsync(
                Client,
                familyId,
                ChildMemberName,
                MemberRole.Child);

        var parentMemberId =
            await IntegrationTestHelpers.AddMemberAsync(
                Client,
                familyId,
                ParentMemberName,
                MemberRole.Parent);

        var activityId =
            await IntegrationTestHelpers.CreateActivityAsync(
                Client,
                familyId,
                childMemberId,
                ActivityTitle,
                ActivityDescription,
                ActivityCategory,
                ActivityPoints,
                null,
                true);

        var completionId =
            await IntegrationTestHelpers.CompleteActivityAsync(
                Client,
                activityId,
                childMemberId);

        var command = new ApproveCompletionCommand(
            Guid.NewGuid(),
            parentMemberId);

        // Act
        var response = await Client.PostAsJsonAsync(
            $"/api/completions/{completionId}/approve",
            command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ApproveCompletionWithEmptyApprovingMemberIdReturnsBadRequest()
    {
        // Arrange
        var familyId =
            await IntegrationTestHelpers.CreateFamilyAsync(
                Client,
                FamilyName);

        var childMemberId =
            await IntegrationTestHelpers.AddMemberAsync(
                Client,
                familyId,
                ChildMemberName,
                MemberRole.Child);

        var activityId =
            await IntegrationTestHelpers.CreateActivityAsync(
                Client,
                familyId,
                childMemberId,
                ActivityTitle,
                ActivityDescription,
                ActivityCategory,
                ActivityPoints,
                null,
                true);

        var completionId =
            await IntegrationTestHelpers.CompleteActivityAsync(
                Client,
                activityId,
                childMemberId);

        var command = new ApproveCompletionCommand(
            completionId,
            Guid.Empty);

        // Act
        var response = await Client.PostAsJsonAsync(
            $"/api/completions/{completionId}/approve",
            command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ApproveCompletionWithNonExistentCompletionReturnsInternalServerError()
    {
        // Arrange
        var familyId =
            await IntegrationTestHelpers.CreateFamilyAsync(
                Client,
                FamilyName);

        var parentMemberId =
            await IntegrationTestHelpers.AddMemberAsync(
                Client,
                familyId,
                ParentMemberName,
                MemberRole.Parent);

        var completionId = Guid.NewGuid();

        var command = new ApproveCompletionCommand(
            completionId,
            parentMemberId);

        // Act
        var response = await Client.PostAsJsonAsync(
            $"/api/completions/{completionId}/approve",
            command);

        // Assert
        response.StatusCode.Should().Be(
            HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task ApproveCompletionWithChildAsApprovingMemberReturnsInternalServerError()
    {
        // Arrange
        var familyId =
            await IntegrationTestHelpers.CreateFamilyAsync(
                Client,
                FamilyName);

        var childMemberId =
            await IntegrationTestHelpers.AddMemberAsync(
                Client,
                familyId,
                ChildMemberName,
                MemberRole.Child);

        var activityId =
            await IntegrationTestHelpers.CreateActivityAsync(
                Client,
                familyId,
                childMemberId,
                ActivityTitle,
                ActivityDescription,
                ActivityCategory,
                ActivityPoints,
                null,
                true);

        var completionId =
            await IntegrationTestHelpers.CompleteActivityAsync(
                Client,
                activityId,
                childMemberId);

        var command = new ApproveCompletionCommand(
            completionId,
            childMemberId);

        // Act
        var response = await Client.PostAsJsonAsync(
            $"/api/completions/{completionId}/approve",
            command);

        // Assert
        response.StatusCode.Should().Be(
            HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task ApproveCompletionWithApprovingMemberFromDifferentFamilyReturnsInternalServerError()
    {
        // Arrange
        var familyId =
            await IntegrationTestHelpers.CreateFamilyAsync(
                Client,
                FamilyName);

        var childMemberId =
            await IntegrationTestHelpers.AddMemberAsync(
                Client,
                familyId,
                ChildMemberName,
                MemberRole.Child);

        var activityId =
            await IntegrationTestHelpers.CreateActivityAsync(
                Client,
                familyId,
                childMemberId,
                ActivityTitle,
                ActivityDescription,
                ActivityCategory,
                ActivityPoints,
                null,
                true);

        var completionId =
            await IntegrationTestHelpers.CompleteActivityAsync(
                Client,
                activityId,
                childMemberId);

        var otherFamilyId =
            await IntegrationTestHelpers.CreateFamilyAsync(
                Client,
                OtherFamilyName);

        var parentMemberId =
            await IntegrationTestHelpers.AddMemberAsync(
                Client,
                otherFamilyId,
                ParentMemberName,
                MemberRole.Parent);

        var command = new ApproveCompletionCommand(
            completionId,
            parentMemberId);

        // Act
        var response = await Client.PostAsJsonAsync(
            $"/api/completions/{completionId}/approve",
            command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task ApproveCompletionWithNonExistentApprovingMemberReturnsInternalServerError()
    {
        // Arrange
        var familyId =
            await IntegrationTestHelpers.CreateFamilyAsync(
                Client,
                FamilyName);

        var childMemberId =
            await IntegrationTestHelpers.AddMemberAsync(
                Client,
                familyId,
                ChildMemberName,
                MemberRole.Child);

        var activityId =
            await IntegrationTestHelpers.CreateActivityAsync(
                Client,
                familyId,
                childMemberId,
                ActivityTitle,
                ActivityDescription,
                ActivityCategory,
                ActivityPoints,
                null,
                true);

        var completionId =
            await IntegrationTestHelpers.CompleteActivityAsync(
                Client,
                activityId,
                childMemberId);

        var command = new ApproveCompletionCommand(
            completionId,
            Guid.NewGuid());

        // Act
        var response = await Client.PostAsJsonAsync(
            $"/api/completions/{completionId}/approve",
            command);

        // Assert
        response.StatusCode.Should().Be(
            HttpStatusCode.InternalServerError);
    }
}