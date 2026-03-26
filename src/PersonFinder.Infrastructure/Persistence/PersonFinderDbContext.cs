using Microsoft.EntityFrameworkCore;
using PersonFinder.Infrastructure.Persistence.Models;

namespace PersonFinder.Infrastructure.Persistence;

public sealed class PersonFinderDbContext : DbContext
{
    public PersonFinderDbContext(DbContextOptions<PersonFinderDbContext> options)
        : base(options)
    {
    }

    public DbSet<PersonDataModel> Persons => Set<PersonDataModel>();
    public DbSet<LocationDataModel> Locations => Set<LocationDataModel>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PersonFinderDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
