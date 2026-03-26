using PersonFinder.Domain.Entities;

namespace PersonFinder.Domain.Repositories;

public interface IPersonRepository
{
    Task<Person?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task AddAsync(Person person, CancellationToken cancellationToken = default);
    Task UpdateBioAsync(long id, string bio, CancellationToken cancellationToken = default);
    Task UpdateLocationAsync(long id, double latitude, double longitude, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Person>> FindNearbyAsync(double latitude, double longitude, double radiusMeters, CancellationToken cancellationToken = default);
}
