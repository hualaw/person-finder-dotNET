namespace PersonFinder.Application.Features.Persons.Queries;

public sealed record GetNearbyPersonsQuery(
    double Latitude,
    double Longitude,
    double RadiusMeters);
