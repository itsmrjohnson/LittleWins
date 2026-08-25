using LittleWins.Domain.Enums;

namespace LittleWins.Domain.Entities;

public class Activity
{
    public Guid Id { get; private set; }

    public Guid FamilyId { get; private set; }

    public string Title { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public string Category { get; private set; } = string.Empty;

    public Guid AssignedToMemberId { get; private set; }

    public int Points { get; private set; }

    public DateTime? DueDate { get; private set; }

    public bool RequiresApproval { get; private set; }

    public ActivityStatus Status { get; private set; }

    public DateTime CreatedAt { get; private set; }

    private Activity()
    {
    }

    public Activity(
        Guid familyId,
        string title,
        string? description,
        string category,
        Guid assignedToMemberId,
        int points,
        DateTime? dueDate,
        bool requiresApproval)
    {
        if (familyId == Guid.Empty)
        {
            throw new ArgumentException(
                "Family ID is required.",
                nameof(familyId));
        }

        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException(
                "Activity title is required.",
                nameof(title));
        }

        if (string.IsNullOrWhiteSpace(category))
        {
            throw new ArgumentException(
                "Activity category is required.",
                nameof(category));
        }

        if (assignedToMemberId == Guid.Empty)
        {
            throw new ArgumentException(
                "Assigned member ID is required.",
                nameof(assignedToMemberId));
        }

        if (points <= 0)
        {
            throw new ArgumentException(
                "Points must be greater than zero.",
                nameof(points));
        }

        Id = Guid.NewGuid();
        FamilyId = familyId;
        Title = title;
        Description = description;
        Category = category;
        AssignedToMemberId = assignedToMemberId;
        Points = points;
        DueDate = dueDate;
        RequiresApproval = requiresApproval;
        Status = ActivityStatus.Active;
        CreatedAt = DateTime.UtcNow;
    }
}