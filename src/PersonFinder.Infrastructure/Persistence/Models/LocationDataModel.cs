namespace PersonFinder.Infrastructure.Persistence.Models;

public sealed class LocationDataModel
{
    public long Id { get; set; }
    public long PersonId { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}
