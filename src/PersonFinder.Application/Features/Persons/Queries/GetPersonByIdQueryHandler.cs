using PersonFinder.Application.Common;
using PersonFinder.Application.DTOs;
using PersonFinder.Domain.Repositories;

namespace PersonFinder.Application.Features.Persons.Queries;

public sealed class GetPersonByIdQueryHandler
{
    private readonly IPersonRepository _personRepository;

    public GetPersonByIdQueryHandler(IPersonRepository personRepository)
    {
        _personRepository = personRepository;
    }

    public async Task<Result<PersonDto>> Handle(
        GetPersonByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var person = await _personRepository.GetByIdAsync(query.PersonId, cancellationToken);
        if (person is null)
        {
            return Result<PersonDto>.Failure("Person not found.");
        }

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
