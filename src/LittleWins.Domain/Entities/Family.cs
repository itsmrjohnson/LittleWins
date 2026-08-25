namespace LittleWins.Domain.Entities;

public class Family
{
    public Guid Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public DateTime CreatedAt { get; private set; }

    public ICollection<Member> Members { get; private set; } = new List<Member>();

    public ICollection<Activity> Activities { get; private set; } = new List<Activity>();

    private Family()
    {
    }

    public Family(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Family name is required.", nameof(name));
        }

        Id = Guid.NewGuid();
        Name = name;
        CreatedAt = DateTime.UtcNow;
    }

    public void AddMember(Member member)
    {
        ArgumentNullException.ThrowIfNull(member);

        if (member.FamilyId != Id)
        {
            throw new InvalidOperationException(
                "Member does not belong to this family.");
        }

        Members.Add(member);
    }

    public void AddActivity(Activity activity)
    {
        ArgumentNullException.ThrowIfNull(activity);

        if (activity.FamilyId != Id)
        {
            throw new InvalidOperationException(
                "Activity does not belong to this family.");
        }

        Activities.Add(activity);
    }
}