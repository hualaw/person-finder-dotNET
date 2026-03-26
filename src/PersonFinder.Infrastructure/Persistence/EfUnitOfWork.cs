using PersonFinder.Application.Abstractions;

namespace PersonFinder.Infrastructure.Persistence;

public sealed class EfUnitOfWork : IUnitOfWork
{
    private readonly PersonFinderDbContext _dbContext;

    public EfUnitOfWork(PersonFinderDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
