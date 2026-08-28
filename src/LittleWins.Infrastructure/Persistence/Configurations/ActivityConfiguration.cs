using LittleWins.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LittleWins.Infrastructure.Persistence.Configurations;

public sealed class ActivityConfiguration
    : IEntityTypeConfiguration<Activity>
{
    public void Configure(EntityTypeBuilder<Activity> builder)
    {
        builder.ToTable("Activities");

        builder.HasKey(activity => activity.Id);

        builder.Property(activity => activity.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(activity => activity.Description)
            .HasMaxLength(1000);

        builder.Property(activity => activity.Category)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(activity => activity.AssignedToMemberId)
            .IsRequired();

        builder.Property(activity => activity.Points)
            .IsRequired();

        builder.Property(activity => activity.DueDate)
            .IsRequired(false);

        builder.Property(activity => activity.RequiresApproval)
            .IsRequired();

        builder.Property(activity => activity.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(activity => activity.CreatedAt)
            .IsRequired();

        builder.HasIndex(activity => activity.FamilyId);

        builder.HasIndex(activity => activity.AssignedToMemberId);
    }
}