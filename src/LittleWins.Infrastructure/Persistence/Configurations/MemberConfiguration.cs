using LittleWins.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LittleWins.Infrastructure.Persistence.Configurations;

public sealed class MemberConfiguration
    : IEntityTypeConfiguration<Member>
{
    public void Configure(EntityTypeBuilder<Member> builder)
    {
        builder.ToTable("Members");

        builder.HasKey(member => member.Id);

        builder.Property(member => member.FamilyId)
            .IsRequired();

        builder.Property(member => member.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(member => member.Role)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(member => member.Points)
            .IsRequired();

        builder.Property(member => member.CreatedAt)
            .IsRequired();

        builder.HasIndex(member => member.FamilyId);
    }
}