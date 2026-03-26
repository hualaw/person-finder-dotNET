using Microsoft.EntityFrameworkCore;
using PersonFinder.Domain.Entities;
using PersonFinder.Domain.Repositories;
using PersonFinder.Domain.ValueObjects;
using PersonFinder.Infrastructure.Persistence.Models;

namespace PersonFinder.Infrastructure.Persistence.Repositories;

public sealed class PersonRepository : IPersonRepository
{
    private readonly PersonFinderDbContext _dbContext;

    public PersonRepository(PersonFinderDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Person?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var data = await _dbContext.Persons
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (data is null)
        {
            return null;
        }

        var locationData = await _dbContext.Locations
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.PersonId == id, cancellationToken);

        GeoLocation? location = null;
        if (locationData is not null)
        {
            location = GeoLocation.Create(locationData.Latitude, locationData.Longitude);
        }

        return Person.Rehydrate(data.Id, data.Name, data.JobTitle, data.Bio, data.Hobbies, location);
    }

    public async Task AddAsync(Person person, CancellationToken cancellationToken = default)
    {
        var data = new PersonDataModel
        {
            Name = person.Name,
            JobTitle = person.JobTitle,
            Bio = person.Bio,
            Hobbies = person.Hobbies.ToArray()
        };

        await _dbContext.Persons.AddAsync(data, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        person.AssignId(data.Id);

        if (person.Location is not null)
        {
            await UpsertLocationAsync(data.Id, person.Location.Value.Latitude, person.Location.Value.Longitude, cancellationToken);
        }
    }

    public async Task UpdateBioAsync(long id, string bio, CancellationToken cancellationToken = default)
    {
        await _dbContext.Persons
            .Where(x => x.Id == id)
            .ExecuteUpdateAsync(s => s.SetProperty(x => x.Bio, bio), cancellationToken);
    }

    public async Task UpdateLocationAsync(
        long id,
        double latitude,
        double longitude,
        CancellationToken cancellationToken = default)
    {
        await UpsertLocationAsync(id, latitude, longitude, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Person>> FindNearbyAsync(
        double latitude,
        double longitude,
        double radiusMeters,
        CancellationToken cancellationToken = default)
    {
        var locationRows = await _dbContext.Locations
            .FromSqlInterpolated($"""
                SELECT id, person_id, latitude, longitude FROM locations
                WHERE ST_DWithin(
                    geom,
                                        ST_SetSRID(ST_MakePoint({longitude}, {latitude}), 4326)::geography,
                                        {radiusMeters}
                )
                """)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        if (locationRows.Count == 0)
        {
            return Array.Empty<Person>();
        }

        var personIds = locationRows.Select(x => x.PersonId).ToArray();
        var persons = await _dbContext.Persons
            .AsNoTracking()
            .Where(x => personIds.Contains(x.Id))
            .ToListAsync(cancellationToken);

        var locationMap = locationRows.ToDictionary(x => x.PersonId);

        return persons.Select(data =>
        {
            GeoLocation? location = null;
            if (locationMap.TryGetValue(data.Id, out var locationData))
            {
                location = GeoLocation.Create(locationData.Latitude, locationData.Longitude);
            }

            return Person.Rehydrate(data.Id, data.Name, data.JobTitle, data.Bio, data.Hobbies, location);
        }).ToList();
    }

    private Task<int> UpsertLocationAsync(
        long personId,
        double latitude,
        double longitude,
        CancellationToken cancellationToken)
    {
        return _dbContext.Database.ExecuteSqlInterpolatedAsync($"""
            INSERT INTO locations (person_id, latitude, longitude, geom)
            VALUES (
                {personId},
                {latitude},
                {longitude},
                ST_SetSRID(ST_MakePoint({longitude}, {latitude}), 4326)::geography
            )
            ON CONFLICT (person_id)
            DO UPDATE SET
                latitude = EXCLUDED.latitude,
                longitude = EXCLUDED.longitude,
                geom = EXCLUDED.geom
            """, cancellationToken);
    }
}
