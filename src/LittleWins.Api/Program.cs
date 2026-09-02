using Microsoft.EntityFrameworkCore;
using LittleWins.Application.UseCases.Families.CreateFamily;
using LittleWins.Application.UseCases.Members.AddMember;
using LittleWins.Application.UseCases.Activities.CreateActivity;
using LittleWins.Application.UseCases.Completions.CompleteActivity;
using LittleWins.Application.UseCases.Completions.ApproveCompletion;
using LittleWins.Application.Abstractions.Persistence;
using LittleWins.Infrastructure.Persistence;
using LittleWins.Infrastructure.Persistence.Repositories;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<LittleWinsDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("LittleWins")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<CreateFamilyHandler>();
builder.Services.AddScoped<AddMemberHandler>();
builder.Services.AddScoped<CreateActivityHandler>();
builder.Services.AddScoped<CompleteActivityHandler>();
builder.Services.AddScoped<ApproveCompletionHandler>();

builder.Services.AddScoped<IFamilyRepository, FamilyRepository>();
builder.Services.AddScoped<IMemberRepository, MemberRepository>();
builder.Services.AddScoped<IActivityRepository, ActivityRepository>();
builder.Services.AddScoped<IActivityCompletionRepository, ActivityCompletionRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();
