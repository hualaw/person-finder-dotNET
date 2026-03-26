using PersonFinder.Application.Abstractions;
using PersonFinder.Application.Common;
using PersonFinder.Domain.Repositories;

namespace PersonFinder.Application.Features.Persons.Commands;

public sealed class UpdatePersonLocationCommandHandler
{
    private readonly IPersonRepository _personRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdatePersonLocationCommandHandler(
        IPersonRepository personRepository,
        IUnitOfWork unitOfWork)
    {
        _personRepository = personRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(
        UpdatePersonLocationCommand command,
        CancellationToken cancellationToken = default)
    {
        var person = await _personRepository.GetByIdAsync(command.PersonId, cancellationToken);
        if (person is null)
        {
            return Result<bool>.Failure("Person not found.");
        }

        await _personRepository.UpdateLocationAsync(
            command.PersonId,
            command.Latitude,
            command.Longitude,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
