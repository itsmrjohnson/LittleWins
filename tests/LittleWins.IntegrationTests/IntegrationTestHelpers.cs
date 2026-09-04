using System.Net.Http.Json;
using FluentAssertions;
using LittleWins.Application.UseCases.Activities.CreateActivity;
using LittleWins.Application.UseCases.Completions.CompleteActivity;
using LittleWins.Application.UseCases.Families.CreateFamily;
using LittleWins.Application.UseCases.Members.AddMember;
using LittleWins.Domain.Enums;

namespace LittleWins.IntegrationTests;

public static class IntegrationTestHelpers
{
    public static async Task<Guid> CreateFamilyAsync(
        HttpClient client,
        string familyName)
    {
        var command = new CreateFamilyCommand(familyName);

        var response = await client.PostAsJsonAsync(
            "/api/families",
            command);

        response.EnsureSuccessStatusCode();

        var result = await response.Content
            .ReadFromJsonAsync<CreateFamilyResult>();

        result.Should().NotBeNull();

        return result!.FamilyId;
    }

    public static async Task<Guid> AddMemberAsync(
        HttpClient client,
        Guid familyId,
        string memberName,
        MemberRole role)
    {
        var command = new AddMemberCommand(
            familyId,
            memberName,
            role);

        var response = await client.PostAsJsonAsync(
            $"/api/families/{familyId}/members",
            command);

        response.EnsureSuccessStatusCode();

        var result = await response.Content
            .ReadFromJsonAsync<AddMemberResult>();

        result.Should().NotBeNull();

        return result!.MemberId;
    }

    public static async Task<Guid> CreateActivityAsync(
        HttpClient client,
        Guid familyId,
        Guid memberId,
        string title,
        string? description,
        string category,
        int points,
        DateTime? dueDate,
        bool requiresApproval)
    {
        var command = new CreateActivityCommand(
            familyId,
            title,
            description,
            category,
            memberId,
            points,
            dueDate,
            requiresApproval);

        var response = await client.PostAsJsonAsync(
            "/api/activities",
            command);

        response.EnsureSuccessStatusCode();

        var result = await response.Content
            .ReadFromJsonAsync<CreateActivityResult>();

        result.Should().NotBeNull();

        return result!.ActivityId;
    }

    public static async Task<Guid> CompleteActivityAsync(
        HttpClient client,
        Guid activityId,
        Guid memberId)
    {
        var command = new CompleteActivityCommand(
            activityId,
            memberId);

        var response = await client.PostAsJsonAsync(
            $"/api/activities/{activityId}/complete",
            command);

        response.EnsureSuccessStatusCode();

        var result = await response.Content
            .ReadFromJsonAsync<CompleteActivityResult>();

        result.Should().NotBeNull();

        return result!.CompletionId;
    }
}