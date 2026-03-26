using PersonFinder.Domain.Common;
using PersonFinder.Domain.Events;
using PersonFinder.Domain.ValueObjects;

namespace PersonFinder.Domain.Entities;

public sealed class Person : AggregateRoot
{
    public long Id { get; private set; }
    public string Name { get; private set; }
    public string JobTitle { get; private set; }
    public string Bio { get; private set; }
    public IReadOnlyCollection<string> Hobbies => _hobbies.AsReadOnly();
    public GeoLocation? Location { get; private set; }

    private readonly List<string> _hobbies;

    private Person(long id, string name, string jobTitle, string bio, List<string> hobbies, GeoLocation? location)
    {
        Id = id;
        Name = name;
        JobTitle = jobTitle;
        Bio = bio;
        _hobbies = hobbies;
        Location = location;
    }

    public static Person Create(string name, string jobTitle, IEnumerable<string> hobbies, GeoLocation? location = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name is required.", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(jobTitle))
        {
            throw new ArgumentException("Job title is required.", nameof(jobTitle));
        }

        var person = new Person(0, name.Trim(), jobTitle.Trim(), string.Empty, hobbies.Distinct().ToList(), location);
        person.Raise(new PersonCreatedDomainEvent(person.Id));
        return person;
    }

    public static Person Rehydrate(long id, string name, string jobTitle, string bio, IEnumerable<string> hobbies, GeoLocation? location = null)
    {
        if (id <= 0)
        {
            throw new ArgumentException("Id is required.", nameof(id));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name is required.", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(jobTitle))
        {
            throw new ArgumentException("Job title is required.", nameof(jobTitle));
        }

        return new Person(id, name.Trim(), jobTitle.Trim(), bio, hobbies.Distinct().ToList(), location);
    }

    public void AssignId(long id)
    {
        if (id <= 0)
        {
            throw new ArgumentException("Id is required.", nameof(id));
        }

        Id = id;
    }

    public void UpdateLocation(GeoLocation location)
    {
        Location = location;
    }

    public void UpdateBio(string bio)
    {
        Bio = bio ?? string.Empty;
    }
}
