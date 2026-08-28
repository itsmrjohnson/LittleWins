using Microsoft.Extensions.DependencyInjection;
using LittleWins.Application.UseCases.Activities.CreateActivity;
using LittleWins.Application.UseCases.Completions.ApproveCompletion;
using LittleWins.Application.UseCases.Completions.CompleteActivity;
using LittleWins.Application.UseCases.Families.CreateFamily;
using LittleWins.Application.UseCases.Members.AddMember;

namespace LittleWins.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddScoped<CreateFamilyHandler>();
        services.AddScoped<AddMemberHandler>();
        services.AddScoped<CreateActivityHandler>();
        services.AddScoped<CompleteActivityHandler>();
        services.AddScoped<ApproveCompletionHandler>();

        return services;
    }
}