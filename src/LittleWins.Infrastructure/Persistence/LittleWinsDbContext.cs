using LittleWins.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LittleWins.Infrastructure.Persistence;

public sealed class LittleWinsDbContext : DbContext
{
    public LittleWinsDbContext(
        DbContextOptions<LittleWinsDbContext> options)
        : base(options)
    {
    }

    public DbSet<Family> Families => Set<Family>();

    public DbSet<Member> Members => Set<Member>();

    public DbSet<Activity> Activities => Set<Activity>();

    public DbSet<ActivityCompletion> ActivityCompletions =>
        Set<ActivityCompletion>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(LittleWinsDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}