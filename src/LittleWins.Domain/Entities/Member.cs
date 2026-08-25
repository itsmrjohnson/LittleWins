using LittleWins.Domain.Enums;

namespace LittleWins.Domain.Entities;

public class Member
{
    public Guid Id { get; private set; }

    public Guid FamilyId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public MemberRole Role { get; private set; }

    public int Points { get; private set; }

    public DateTime CreatedAt { get; private set; }

    private Member()
    {
    }

    public Member(Guid familyId, string name, MemberRole role)
    {
        if (familyId == Guid.Empty)
        {
            throw new ArgumentException("Family ID is required.", nameof(familyId));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Member name is required.", nameof(name));
        }

        Id = Guid.NewGuid();
        FamilyId = familyId;
        Name = name;
        Role = role;
        Points = 0;
        CreatedAt = DateTime.UtcNow;
    }

    public void AwardPoints(int points)
    {
        if (points <= 0)
        {
            throw new ArgumentException(
                "Points must be greater than zero.",
                nameof(points));
        }

        Points += points;
    }
}