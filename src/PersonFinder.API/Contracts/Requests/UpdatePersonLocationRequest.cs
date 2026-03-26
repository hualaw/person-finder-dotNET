namespace PersonFinder.API.Contracts.Requests;

public sealed record UpdatePersonLocationRequest(
    double Latitude,
    double Longitude);
