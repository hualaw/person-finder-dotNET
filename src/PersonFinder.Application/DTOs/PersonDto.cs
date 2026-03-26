namespace PersonFinder.Application.DTOs;

public sealed record PersonDto(
    long Id,
    string Name,
    string JobTitle,
    string Bio,
    IReadOnlyCollection<string> Hobbies,
    double? Latitude,
    double? Longitude);
