using PersonFinder.Domain.Entities;
using PersonFinder.Domain.ValueObjects;

namespace PersonFinder.Domain.Tests.Entities;

public sealed class PersonEntityTests
{
    [Fact]
    public void Create_WithValidInput_ReturnsPersonWithCorrectData()
    {
        // Arrange
        var name = "John Doe";
        var jobTitle = "Software Engineer";
        var hobbies = new[] { "coding", "music", "reading" };

        // Act
        var person = Person.Create(name, jobTitle, hobbies);

        // Assert
        Assert.Equal(name, person.Name);
        Assert.Equal(jobTitle, person.JobTitle);
        Assert.Equal(hobbies.Length, person.Hobbies.Count);
        Assert.Empty(person.Bio);
        Assert.Null(person.Location);
        Assert.Equal(0, person.Id); // Not assigned yet
    }

    [Fact]
    public void Create_WithLocation_ReturnsPersonWithLocation()
    {
        // Arrange
        var location = GeoLocation.Create(40.7128, -74.0060); // NYC coordinates

        // Act
        var person = Person.Create("Jane Doe", "Product Manager", new[] { "travel" }, location);

        // Assert
        Assert.NotNull(person.Location);
        Assert.Equal(40.7128, person.Location.Value.Latitude);
        Assert.Equal(-74.0060, person.Location.Value.Longitude);
    }

    [Fact]
    public void Create_WithNullName_ThrowsArgumentException()
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            Person.Create(null!, "Engineer", new[] { "hobby" }));
        Assert.Contains("Name is required", ex.Message);
    }

    [Fact]
    public void Create_WithWhitespaceName_ThrowsArgumentException()
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            Person.Create("   ", "Engineer", new[] { "hobby" }));
        Assert.Contains("Name is required", ex.Message);
    }

    [Fact]
    public void Create_WithNullJobTitle_ThrowsArgumentException()
    {
        // Act & Assert
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        var ex = Assert.Throws<ArgumentException>(() =>
            Person.Create("John", null, new[] { "hobby" }));
#pragma warning restore CS8625
        Assert.Contains("Job title is required", ex.Message);
    }

    [Fact]
    public void Create_WithDuplicateHobbies_DeduplicatesHobbies()
    {
        // Arrange
        var hobbies = new[] { "coding", "music", "coding", "reading", "music" };

        // Act
        var person = Person.Create("John", "Engineer", hobbies);

        // Assert
        Assert.Equal(3, person.Hobbies.Count);
        Assert.Contains("coding", person.Hobbies);
        Assert.Contains("music", person.Hobbies);
        Assert.Contains("reading", person.Hobbies);
    }

    [Fact]
    public void Rehydrate_WithValidId_ReturnsPersonWithId()
    {
        // Act
        var person = Person.Rehydrate(1, "John", "Engineer", "A bio", new[] { "hobby" });

        // Assert
        Assert.Equal(1, person.Id);
        Assert.Equal("John", person.Name);
        Assert.Equal("A bio", person.Bio);
    }

    [Fact]
    public void Rehydrate_WithNegativeId_ThrowsArgumentException()
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            Person.Rehydrate(-1, "John", "Engineer", "bio", new[] { "hobby" }));
        Assert.Contains("Id is required", ex.Message);
    }

    [Fact]
    public void Rehydrate_WithZeroId_ThrowsArgumentException()
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            Person.Rehydrate(0, "John", "Engineer", "bio", new[] { "hobby" }));
        Assert.Contains("Id is required", ex.Message);
    }

    [Fact]
    public void AssignId_WithPositiveId_AssignsId()
    {
        // Arrange
        var person = Person.Create("John", "Engineer", new[] { "hobby" });

        // Act
        person.AssignId(42);

        // Assert
        Assert.Equal(42, person.Id);
    }

    [Fact]
    public void AssignId_WithZeroId_ThrowsArgumentException()
    {
        // Arrange
        var person = Person.Create("John", "Engineer", new[] { "hobby" });

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => person.AssignId(0));
        Assert.Contains("Id is required", ex.Message);
    }

    [Fact]
    public void UpdateBio_WithNewBio_UpdatesBio()
    {
        // Arrange
        var person = Person.Rehydrate(1, "John", "Engineer", "", new[] { "hobby" });
        var newBio = "Updated bio description";

        // Act
        person.UpdateBio(newBio);

        // Assert
        Assert.Equal(newBio, person.Bio);
    }

    [Fact]
    public void UpdateBio_WithNull_SetsBioToEmpty()
    {
        // Arrange
        var person = Person.Rehydrate(1, "John", "Engineer", "old bio", new[] { "hobby" });

        // Act
        person.UpdateBio(null);

        // Assert
        Assert.Empty(person.Bio);
    }

    [Fact]
    public void UpdateLocation_WithNewLocation_UpdatesLocation()
    {
        // Arrange
        var person = Person.Rehydrate(1, "John", "Engineer", "bio", new[] { "hobby" });
        var newLocation = GeoLocation.Create(51.5074, -0.1278); // London

        // Act
        person.UpdateLocation(newLocation);

        // Assert
        Assert.NotNull(person.Location);
        Assert.Equal(51.5074, person.Location.Value.Latitude);
        Assert.Equal(-0.1278, person.Location.Value.Longitude);
    }

    [Fact]
    public void Create_TrimsNameAndJobTitle()
    {
        // Act
        var person = Person.Create("  John  ", "  Engineer  ", new[] { "hobby" });

        // Assert
        Assert.Equal("John", person.Name);
        Assert.Equal("Engineer", person.JobTitle);
    }
}
