namespace PersonFinder.Application.Features.Persons.Commands;

public sealed record UpdatePersonLocationCommand(
    long PersonId,
    double Latitude,
    double Longitude);
