namespace PersonFinder.API.Contracts.Requests;

public sealed record CreatePersonRequest(
    string Name,
    string JobTitle,
    IReadOnlyCollection<string> Hobbies,
    double? Latitude,
    double? Longitude);
