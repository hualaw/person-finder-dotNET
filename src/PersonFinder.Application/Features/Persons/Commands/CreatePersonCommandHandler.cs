using PersonFinder.Application.Abstractions;
using PersonFinder.Application.Common;
using PersonFinder.Application.DTOs;
using PersonFinder.Domain.Entities;
using PersonFinder.Domain.Repositories;
using PersonFinder.Domain.ValueObjects;

namespace PersonFinder.Application.Features.Persons.Commands;

public sealed class CreatePersonCommandHandler
{
    private readonly IPersonRepository _personRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBioQueue _bioQueue;

    public CreatePersonCommandHandler(
        IPersonRepository personRepository,
        IUnitOfWork unitOfWork,
        IBioQueue bioQueue)
    {
        _personRepository = personRepository;
        _unitOfWork = unitOfWork;
        _bioQueue = bioQueue;
    }

    public async Task<Result<PersonDto>> Handle(CreatePersonCommand command, CancellationToken cancellationToken = default)
    {
        GeoLocation? location = null;
        if (command.Latitude.HasValue && command.Longitude.HasValue)
        {
            location = GeoLocation.Create(command.Latitude.Value, command.Longitude.Value);
        }

        var person = Person.Create(command.Name, command.JobTitle, command.Hobbies, location);
        await _personRepository.AddAsync(person, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Fire-and-forget: enqueue async bio generation (mirrors Spring @Async event listener)
        _bioQueue.Enqueue(new BioGenerationItem(person.Id, person.JobTitle, person.Hobbies));

        var dto = new PersonDto(
            person.Id,
            person.Name,
            person.JobTitle,
            person.Bio,
            person.Hobbies,
            person.Location?.Latitude,
            person.Location?.Longitude);

        return Result<PersonDto>.Success(dto);
    }
}
