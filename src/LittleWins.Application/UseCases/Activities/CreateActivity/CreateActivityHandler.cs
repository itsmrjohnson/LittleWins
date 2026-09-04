using FluentValidation;
using LittleWins.Application.Abstractions.Persistence;
using LittleWins.Domain.Entities;

namespace LittleWins.Application.UseCases.Activities.CreateActivity;

public sealed class CreateActivityHandler
{
    private readonly IFamilyRepository _familyRepository;
    private readonly IMemberRepository _memberRepository;
    private readonly IActivityRepository _activityRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateActivityCommand> _validator;

    public CreateActivityHandler(
        IFamilyRepository familyRepository,
        IMemberRepository memberRepository,
        IActivityRepository activityRepository,
        IUnitOfWork unitOfWork,
        IValidator<CreateActivityCommand> validator)
    {
        _familyRepository = familyRepository;
        _memberRepository = memberRepository;
        _activityRepository = activityRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<CreateActivityResult> HandleAsync(
        CreateActivityCommand command,
        CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(
            command,
            cancellationToken);

        var family = await _familyRepository.GetByIdAsync(
            command.FamilyId,
            cancellationToken);

        if (family is null)
        {
            throw new InvalidOperationException(
                "Family was not found.");
        }

        var member = await _memberRepository.GetByIdAsync(
            command.AssignedToMemberId,
            cancellationToken);

        if (member is null)
        {
            throw new InvalidOperationException(
                "Assigned member was not found.");
        }

        if (member.FamilyId != family.Id)
        {
            throw new InvalidOperationException(
                "Assigned member does not belong to the family.");
        }

        var activity = new Activity(
            family.Id,
            command.Title,
            command.Description,
            command.Category,
            member.Id,
            command.Points,
            command.DueDate,
            command.RequiresApproval);

        family.AddActivity(activity);

        await _activityRepository.AddAsync(
            activity,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return new CreateActivityResult(
            activity.Id,
            activity.FamilyId,
            activity.Title,
            activity.AssignedToMemberId,
            activity.Points);
    }
}