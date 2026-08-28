using LittleWins.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LittleWins.Infrastructure.Persistence.Configurations;

public sealed class FamilyConfiguration
    : IEntityTypeConfiguration<Family>
{
    public void Configure(EntityTypeBuilder<Family> builder)
    {
        builder.ToTable("Families");

        builder.HasKey(family => family.Id);

        builder.Property(family => family.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(family => family.CreatedAt)
            .IsRequired();

        builder.HasMany(family => family.Members)
            .WithOne()
            .HasForeignKey(member => member.FamilyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(family => family.Activities)
            .WithOne()
            .HasForeignKey(activity => activity.FamilyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}