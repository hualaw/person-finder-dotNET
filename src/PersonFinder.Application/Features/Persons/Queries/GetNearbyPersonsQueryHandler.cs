using PersonFinder.Application.Common;
using PersonFinder.Application.DTOs;
using PersonFinder.Domain.Repositories;

namespace PersonFinder.Application.Features.Persons.Queries;

public sealed class GetNearbyPersonsQueryHandler
{
    private readonly IPersonRepository _personRepository;

    public GetNearbyPersonsQueryHandler(IPersonRepository personRepository)
    {
        _personRepository = personRepository;
    }

    public async Task<Result<IReadOnlyCollection<PersonDto>>> Handle(
        GetNearbyPersonsQuery query,
        CancellationToken cancellationToken = default)
    {
        var persons = await _personRepository.FindNearbyAsync(
            query.Latitude,
            query.Longitude,
            query.RadiusMeters,
            cancellationToken);

        var dtos = persons.Select(p => new PersonDto(
            p.Id,
            p.Name,
            p.JobTitle,
            p.Bio,
            p.Hobbies,
            p.Location?.Latitude,
            p.Location?.Longitude))
            .ToList();

        return Result<IReadOnlyCollection<PersonDto>>.Success(dtos);
    }
}
