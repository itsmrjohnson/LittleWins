using FluentValidation;
using LittleWins.Application.Abstractions.Persistence;
using LittleWins.Domain.Entities;

namespace LittleWins.Application.UseCases.Completions.CompleteActivity;

public sealed class CompleteActivityHandler
{
    private readonly IActivityRepository _activityRepository;
    private readonly IMemberRepository _memberRepository;
    private readonly IActivityCompletionRepository _completionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CompleteActivityCommand> _validator;

    public CompleteActivityHandler(
        IActivityRepository activityRepository,
        IMemberRepository memberRepository,
        IActivityCompletionRepository completionRepository,
        IUnitOfWork unitOfWork,
        IValidator<CompleteActivityCommand> validator)
    {
        _activityRepository = activityRepository;
        _memberRepository = memberRepository;
        _completionRepository = completionRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<CompleteActivityResult> HandleAsync(
        CompleteActivityCommand command,
        CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(
            command,
            cancellationToken);

        var activity = await _activityRepository.GetByIdAsync(
            command.ActivityId,
            cancellationToken);

        if (activity is null)
        {
            throw new InvalidOperationException(
                "Activity was not found.");
        }

        var member = await _memberRepository.GetByIdAsync(
            command.MemberId,
            cancellationToken);

        if (member is null)
        {
            throw new InvalidOperationException(
                "Member was not found.");
        }

        if (member.FamilyId != activity.FamilyId)
        {
            throw new InvalidOperationException(
                "Member does not belong to the activity family.");
        }

        if (activity.AssignedToMemberId != member.Id)
        {
            throw new InvalidOperationException(
                "Member is not assigned to this activity.");
        }

        var completion = new ActivityCompletion(
            activity.Id,
            member.Id,
            activity.FamilyId);

        await _completionRepository.AddAsync(
            completion,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return new CompleteActivityResult(
            completion.Id,
            completion.ActivityId,
            completion.MemberId,
            completion.Status.ToString());
    }
}