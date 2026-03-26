using Moq;
using PersonFinder.Application.Features.Persons.Queries;
using PersonFinder.Domain.Entities;
using PersonFinder.Domain.Repositories;
using PersonFinder.Domain.ValueObjects;

namespace PersonFinder.Application.Tests.Features;

public sealed class GetPersonByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingPerson_ReturnsSuccessResult()
    {
        // Arrange
        var personId = 1L;
        var location = GeoLocation.Create(40.7128, -74.0060);
        var person = Person.Rehydrate(
            personId,
            "John Doe",
            "Software Engineer",
            "An experienced developer",
            new[] { "coding", "music" },
            location);

        var mockRepository = new Mock<IPersonRepository>();
        mockRepository.Setup(r => r.GetByIdAsync(personId, default))
            .ReturnsAsync(person);

        var handler = new GetPersonByIdQueryHandler(mockRepository.Object);
        var query = new GetPersonByIdQuery(personId);

        // Act
        var result = await handler.Handle(query);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(personId, result.Value.Id);
        Assert.Equal("John Doe", result.Value.Name);
        Assert.Equal("Software Engineer", result.Value.JobTitle);
        Assert.Equal("An experienced developer", result.Value.Bio);
        Assert.Equal(40.7128, result.Value.Latitude);
        Assert.Equal(-74.0060, result.Value.Longitude);
        mockRepository.Verify(r => r.GetByIdAsync(personId, default), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistingPerson_ReturnsFailureResult()
    {
        // Arrange
        var mockRepository = new Mock<IPersonRepository>();
        mockRepository.Setup(r => r.GetByIdAsync(It.IsAny<long>(), default))
            .ReturnsAsync((Person?)null);

        var handler = new GetPersonByIdQueryHandler(mockRepository.Object);
        var query = new GetPersonByIdQuery(999L);

        // Act
        var result = await handler.Handle(query);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Null(result.Value);
        Assert.Equal("Person not found.", result.Error);
    }

    [Fact]
    public async Task Handle_WithPersonWithoutLocation_ReturnsDtoWithNullLocation()
    {
        // Arrange
        var personId = 2L;
        var person = Person.Rehydrate(
            personId,
            "Jane Doe",
            "Product Manager",
            "PM experience",
            new[] { "travel" },
            null);  // No location

        var mockRepository = new Mock<IPersonRepository>();
        mockRepository.Setup(r => r.GetByIdAsync(personId, default))
            .ReturnsAsync(person);

        var handler = new GetPersonByIdQueryHandler(mockRepository.Object);
        var query = new GetPersonByIdQuery(personId);

        // Act
        var result = await handler.Handle(query);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Null(result.Value?.Latitude);
        Assert.Null(result.Value?.Longitude);
    }

    [Fact]
    public async Task Handle_WithEmptyBio_ReturnsDtoWithEmptyBio()
    {
        // Arrange
        var personId = 3L;
        var person = Person.Rehydrate(
            personId,
            "Bob Smith",
            "Designer",
            "",  // Empty bio
            new[] { "art" });

        var mockRepository = new Mock<IPersonRepository>();
        mockRepository.Setup(r => r.GetByIdAsync(personId, default))
            .ReturnsAsync(person);

        var handler = new GetPersonByIdQueryHandler(mockRepository.Object);
        var query = new GetPersonByIdQuery(personId);

        // Act
        var result = await handler.Handle(query);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value!.Bio);
    }

    [Fact]
    public async Task Handle_WithMultipleHobbies_ReturnsDtoWithAllHobbies()
    {
        // Arrange
        var personId = 4L;
        var hobbies = new[] { "coding", "gaming", "reading", "cooking" };
        var person = Person.Rehydrate(
            personId,
            "Alice",
            "Developer",
            "bio",
            hobbies);

        var mockRepository = new Mock<IPersonRepository>();
        mockRepository.Setup(r => r.GetByIdAsync(personId, default))
            .ReturnsAsync(person);

        var handler = new GetPersonByIdQueryHandler(mockRepository.Object);
        var query = new GetPersonByIdQuery(personId);

        // Act
        var result = await handler.Handle(query);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(4, result.Value!.Hobbies.Count);
        Assert.Contains("coding", result.Value.Hobbies);
        Assert.Contains("gaming", result.Value.Hobbies);
        Assert.Contains("reading", result.Value.Hobbies);
        Assert.Contains("cooking", result.Value.Hobbies);
    }

    [Fact]
    public async Task Handle_PassesCancellationTokenToRepository()
    {
        // Arrange
        var mockRepository = new Mock<IPersonRepository>();
        mockRepository.Setup(r => r.GetByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Person?)null);

        var handler = new GetPersonByIdQueryHandler(mockRepository.Object);
        var query = new GetPersonByIdQuery(1L);
        var cts = new CancellationTokenSource();

        // Act
        await handler.Handle(query, cts.Token);

        // Assert
        mockRepository.Verify(
            r => r.GetByIdAsync(1L, cts.Token),
            Times.Once);
    }

    [Fact]
    public async Task Handle_RepositoryThrowsException_PropagatesException()
    {
        // Arrange
        var mockRepository = new Mock<IPersonRepository>();
        mockRepository.Setup(r => r.GetByIdAsync(It.IsAny<long>(), default))
            .ThrowsAsync(new InvalidOperationException("DB error"));

        var handler = new GetPersonByIdQueryHandler(mockRepository.Object);
        var query = new GetPersonByIdQuery(1L);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(query));
    }
}
