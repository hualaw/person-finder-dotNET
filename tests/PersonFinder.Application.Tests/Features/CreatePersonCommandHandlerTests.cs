using Moq;
using PersonFinder.Application.Abstractions;
using PersonFinder.Application.Common;
using PersonFinder.Application.Features.Persons.Commands;
using PersonFinder.Domain.Entities;
using PersonFinder.Domain.Repositories;

namespace PersonFinder.Application.Tests.Features;

public sealed class CreatePersonCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidCommand_ReturnsSuccessResult()
    {
        // Arrange
        var mockRepository = new Mock<IPersonRepository>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockBioQueue = new Mock<IBioQueue>();

        var handler = new CreatePersonCommandHandler(mockRepository.Object, mockUnitOfWork.Object, mockBioQueue.Object);
        var command = new CreatePersonCommand(
            "John Doe",
            "Software Engineer",
            new[] { "coding", "music" },
            null,
            null);

        // Act
        var result = await handler.Handle(command);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("John Doe", result.Value.Name);
        Assert.Equal("Software Engineer", result.Value.JobTitle);
        Assert.Equal(2, result.Value.Hobbies.Count);
        mockRepository.Verify(r => r.AddAsync(It.IsAny<Person>(), default), Times.Once);
        mockUnitOfWork.Verify(u => u.SaveChangesAsync(default), Times.Once);
        mockBioQueue.Verify(q => q.Enqueue(It.IsAny<BioGenerationItem>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithLocationCoordinates_ReturnsDtoWithLocation()
    {
        // Arrange
        var mockRepository = new Mock<IPersonRepository>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockBioQueue = new Mock<IBioQueue>();

        var handler = new CreatePersonCommandHandler(mockRepository.Object, mockUnitOfWork.Object, mockBioQueue.Object);
        var command = new CreatePersonCommand(
            "Jane Doe",
            "Product Manager",
            new[] { "travel" },
            40.7128,
            -74.0060);

        // Act
        var result = await handler.Handle(command);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(40.7128, result.Value.Latitude);
        Assert.Equal(-74.0060, result.Value.Longitude);
    }

    [Fact]
    public async Task Handle_WithPartialLocationCoordinates_ReturnsNullLocation()
    {
        // Arrange
        var mockRepository = new Mock<IPersonRepository>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockBioQueue = new Mock<IBioQueue>();

        var handler = new CreatePersonCommandHandler(mockRepository.Object, mockUnitOfWork.Object, mockBioQueue.Object);
        var command = new CreatePersonCommand(
            "John",
            "Engineer",
            new[] { "hobby" },
            40.7128,  // Latitude only
            null);    // Longitude null

        // Act
        var result = await handler.Handle(command);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Null(result.Value?.Latitude);
        Assert.Null(result.Value?.Longitude);
    }

    [Fact]
    public async Task Handle_EnqueuesBioGenerationWithCorrectData()
    {
        // Arrange
        var mockRepository = new Mock<IPersonRepository>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockBioQueue = new Mock<IBioQueue>();

        var handler = new CreatePersonCommandHandler(mockRepository.Object, mockUnitOfWork.Object, mockBioQueue.Object);
        var command = new CreatePersonCommand(
            "John Doe",
            "Software Engineer",
            new[] { "coding", "music" },
            null,
            null);

        // Act
        await handler.Handle(command);

        // Assert
        mockBioQueue.Verify(
            q => q.Enqueue(It.Is<BioGenerationItem>(i => 
                i.JobTitle == "Software Engineer" &&
                i.Hobbies.Contains("coding") &&
                i.Hobbies.Contains("music"))),
            Times.Once);
    }

    [Fact]
    public async Task Handle_RepositoryThrowsException_ReturnsFailure()
    {
        // Arrange
        var mockRepository = new Mock<IPersonRepository>();
        mockRepository.Setup(r => r.AddAsync(It.IsAny<Person>(), default))
            .ThrowsAsync(new InvalidOperationException("DB error"));

        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockBioQueue = new Mock<IBioQueue>();

        var handler = new CreatePersonCommandHandler(mockRepository.Object, mockUnitOfWork.Object, mockBioQueue.Object);
        var command = new CreatePersonCommand("John", "Engineer", new[] { "hobby" }, null, null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command));
    }

    [Fact]
    public async Task Handle_CallsRepositoryAddAsyncWithCorrectCancellationToken()
    {
        // Arrange
        var mockRepository = new Mock<IPersonRepository>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockBioQueue = new Mock<IBioQueue>();

        var handler = new CreatePersonCommandHandler(mockRepository.Object, mockUnitOfWork.Object, mockBioQueue.Object);
        var command = new CreatePersonCommand("John", "Engineer", new[] { "hobby" }, null, null);
        var cts = new CancellationTokenSource();

        // Act
        await handler.Handle(command, cts.Token);

        // Assert
        mockRepository.Verify(
            r => r.AddAsync(It.IsAny<Person>(), cts.Token),
            Times.Once);
    }
}
