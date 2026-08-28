using LittleWins.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LittleWins.Infrastructure.Persistence.Configurations;

public sealed class ActivityCompletionConfiguration
    : IEntityTypeConfiguration<ActivityCompletion>
{
    public void Configure(
        EntityTypeBuilder<ActivityCompletion> builder)
    {
        builder.ToTable("ActivityCompletions");

        builder.HasKey(completion => completion.Id);

        builder.Property(completion => completion.ActivityId)
            .IsRequired();

        builder.Property(completion => completion.MemberId)
            .IsRequired();

        builder.Property(completion => completion.FamilyId)
            .IsRequired();

        builder.Property(completion => completion.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(completion => completion.CompletedAt)
            .IsRequired();

        builder.Property(completion => completion.ApprovedAt)
            .IsRequired(false);

        builder.Property(completion => completion.ApprovedByMemberId)
            .IsRequired(false);

        builder.Property(completion => completion.PointsAwarded)
            .IsRequired();

        builder.HasIndex(completion => completion.ActivityId);

        builder.HasIndex(completion => completion.MemberId);

        builder.HasIndex(completion => completion.FamilyId);

        builder.HasIndex(completion => completion.ApprovedByMemberId);
    }
}