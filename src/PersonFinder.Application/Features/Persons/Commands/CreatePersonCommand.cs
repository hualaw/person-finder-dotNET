namespace PersonFinder.Application.Features.Persons.Commands;

public sealed record CreatePersonCommand(
    string Name,
    string JobTitle,
    IReadOnlyCollection<string> Hobbies,
    double? Latitude,
    double? Longitude);
