using LittleWins.Domain.Enums;

namespace LittleWins.Domain.Entities;

public class ActivityCompletion
{
    public Guid Id { get; private set; }

    public Guid ActivityId { get; private set; }

    public Guid MemberId { get; private set; }

    public Guid FamilyId { get; private set; }

    public CompletionStatus Status { get; private set; }

    public DateTime CompletedAt { get; private set; }

    public DateTime? ApprovedAt { get; private set; }

    public Guid? ApprovedByMemberId { get; private set; }

    public bool PointsAwarded { get; private set; }

    private ActivityCompletion()
    {
    }

    public ActivityCompletion(
        Guid activityId,
        Guid memberId,
        Guid familyId)
    {
        if (activityId == Guid.Empty)
        {
            throw new ArgumentException(
                "Activity ID is required.",
                nameof(activityId));
        }

        if (memberId == Guid.Empty)
        {
            throw new ArgumentException(
                "Member ID is required.",
                nameof(memberId));
        }

        if (familyId == Guid.Empty)
        {
            throw new ArgumentException(
                "Family ID is required.",
                nameof(familyId));
        }

        Id = Guid.NewGuid();
        ActivityId = activityId;
        MemberId = memberId;
        FamilyId = familyId;
        Status = CompletionStatus.Pending;
        CompletedAt = DateTime.UtcNow;
        PointsAwarded = false;
    }

    public void Approve(Guid approvedByMemberId)
    {
        if (approvedByMemberId == Guid.Empty)
        {
            throw new ArgumentException(
                "Approving member ID is required.",
                nameof(approvedByMemberId));
        }

        if (Status == CompletionStatus.Approved)
        {
            throw new InvalidOperationException(
                "Completion has already been approved.");
        }

        if (Status == CompletionStatus.Rejected)
        {
            throw new InvalidOperationException(
                "A rejected completion cannot be approved.");
        }

        Status = CompletionStatus.Approved;
        ApprovedAt = DateTime.UtcNow;
        ApprovedByMemberId = approvedByMemberId;
    }

    public void MarkPointsAwarded()
    {
        if (Status != CompletionStatus.Approved)
        {
            throw new InvalidOperationException(
                "Points can only be awarded for an approved completion.");
        }

        if (PointsAwarded)
        {
            throw new InvalidOperationException(
                "Points have already been awarded for this completion.");
        }

        PointsAwarded = true;
    }

    public void Reject()
    {
        if (Status == CompletionStatus.Approved)
        {
            throw new InvalidOperationException(
                "An approved completion cannot be rejected.");
        }

        Status = CompletionStatus.Rejected;
    }
}