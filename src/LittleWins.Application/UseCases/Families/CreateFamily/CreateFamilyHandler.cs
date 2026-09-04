using FluentValidation;
using LittleWins.Application.Abstractions.Persistence;
using LittleWins.Domain.Entities;

namespace LittleWins.Application.UseCases.Families.CreateFamily;

public sealed class CreateFamilyHandler
{
    private readonly IFamilyRepository _familyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateFamilyCommand> _validator;

    public CreateFamilyHandler(
        IFamilyRepository familyRepository,
        IUnitOfWork unitOfWork,
        IValidator<CreateFamilyCommand> validator)
    {
        _familyRepository = familyRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<CreateFamilyResult> HandleAsync(
        CreateFamilyCommand command,
        CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(
            command,
            cancellationToken);

        var family = new Family(command.Name);

        await _familyRepository.AddAsync(
            family,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return new CreateFamilyResult(
            family.Id,
            family.Name);
    }
}