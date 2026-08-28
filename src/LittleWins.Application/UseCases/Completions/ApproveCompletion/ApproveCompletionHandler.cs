using LittleWins.Application.Abstractions.Persistence;

namespace LittleWins.Application.UseCases.Completions.ApproveCompletion;

public sealed class ApproveCompletionHandler
{
    private readonly IActivityCompletionRepository _completionRepository;
    private readonly IActivityRepository _activityRepository;
    private readonly IMemberRepository _memberRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ApproveCompletionHandler(
        IActivityCompletionRepository completionRepository,
        IActivityRepository activityRepository,
        IMemberRepository memberRepository,
        IUnitOfWork unitOfWork)
    {
        _completionRepository = completionRepository;
        _activityRepository = activityRepository;
        _memberRepository = memberRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApproveCompletionResult> HandleAsync(
        ApproveCompletionCommand command,
        CancellationToken cancellationToken)
    {
        var completion = await _completionRepository.GetByIdAsync(
            command.CompletionId,
            cancellationToken);

        if (completion is null)
        {
            throw new InvalidOperationException(
                "Completion was not found.");
        }

        var activity = await _activityRepository.GetByIdAsync(
            completion.ActivityId,
            cancellationToken);

        if (activity is null)
        {
            throw new InvalidOperationException(
                "Activity was not found.");
        }

        var approvingMember = await _memberRepository.GetByIdAsync(
            command.ApprovedByMemberId,
            cancellationToken);

        if (approvingMember is null)
        {
            throw new InvalidOperationException(
                "Approving member was not found.");
        }

        if (approvingMember.FamilyId != completion.FamilyId)
        {
            throw new InvalidOperationException(
                "Approving member does not belong to the family.");
        }

        if (approvingMember.Role != Domain.Enums.MemberRole.Parent)
        {
            throw new InvalidOperationException(
                "Only a parent can approve a completion.");
        }

        var completedByMember = await _memberRepository.GetByIdAsync(
            completion.MemberId,
            cancellationToken);

        if (completedByMember is null)
        {
            throw new InvalidOperationException(
                "Completing member was not found.");
        }

        completion.Approve(approvingMember.Id);

        completedByMember.AwardPoints(activity.Points);

        completion.MarkPointsAwarded();

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return new ApproveCompletionResult(
            completion.Id,
            completedByMember.Id,
            activity.Points);
    }
}