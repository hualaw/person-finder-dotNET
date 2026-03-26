using PersonFinder.Domain.ValueObjects;

namespace PersonFinder.Domain.Tests.ValueObjects;

public sealed class GeoLocationValueObjectTests
{
    [Fact]
    public void Create_WithValidCoordinates_ReturnsGeoLocation()
    {
        // Act
        var location = GeoLocation.Create(40.7128, -74.0060);

        // Assert
        Assert.Equal(40.7128, location.Latitude);
        Assert.Equal(-74.0060, location.Longitude);
    }

    [Fact]
    public void Create_WithMaxLatitude_ReturnsGeoLocation()
    {
        // Act
        var location = GeoLocation.Create(90, 0);

        // Assert
        Assert.Equal(90, location.Latitude);
    }

    [Fact]
    public void Create_WithMinLatitude_ReturnsGeoLocation()
    {
        // Act
        var location = GeoLocation.Create(-90, 0);

        // Assert
        Assert.Equal(-90, location.Latitude);
    }

    [Fact]
    public void Create_WithMaxLongitude_ReturnsGeoLocation()
    {
        // Act
        var location = GeoLocation.Create(0, 180);

        // Assert
        Assert.Equal(180, location.Longitude);
    }

    [Fact]
    public void Create_WithMinLongitude_ReturnsGeoLocation()
    {
        // Act
        var location = GeoLocation.Create(0, -180);

        // Assert
        Assert.Equal(-180, location.Longitude);
    }

    [Fact]
    public void Create_WithLatitudeAbove90_ThrowsArgumentOutOfRangeException()
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() =>
            GeoLocation.Create(91, 0));
        Assert.Contains("Latitude must be between -90 and 90", ex.Message);
    }

    [Fact]
    public void Create_WithLatitudeBelow90_ThrowsArgumentOutOfRangeException()
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() =>
            GeoLocation.Create(-91, 0));
        Assert.Contains("Latitude must be between -90 and 90", ex.Message);
    }

    [Fact]
    public void Create_WithLongitudeAbove180_ThrowsArgumentOutOfRangeException()
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() =>
            GeoLocation.Create(0, 181));
        Assert.Contains("Longitude must be between -180 and 180", ex.Message);
    }

    [Fact]
    public void Create_WithLongitudeBelow180_ThrowsArgumentOutOfRangeException()
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() =>
            GeoLocation.Create(0, -181));
        Assert.Contains("Longitude must be between -180 and 180", ex.Message);
    }

    [Fact]
    public void Create_WithEquatorAndPrimeMeridian_ReturnsGeoLocation()
    {
        // Act
        var location = GeoLocation.Create(0, 0);

        // Assert
        Assert.Equal(0, location.Latitude);
        Assert.Equal(0, location.Longitude);
    }

    [Fact]
    public void GeoLocationRecord_SupportsEquality()
    {
        // Act
        var location1 = GeoLocation.Create(40.7128, -74.0060);
        var location2 = GeoLocation.Create(40.7128, -74.0060);

        // Assert
        Assert.Equal(location1, location2);
    }

    [Fact]
    public void GeoLocationRecord_DifferentCordinates_NotEqual()
    {
        // Act
        var location1 = GeoLocation.Create(40.7128, -74.0060);
        var location2 = GeoLocation.Create(51.5074, -0.1278);

        // Assert
        Assert.NotEqual(location1, location2);
    }
}
