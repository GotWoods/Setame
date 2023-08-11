using Xunit;
using Moq;
using MediatR;
using Setame.Data.Models;
using Setame.Data.Handlers.Applications;
using Setame.Data.Data;
using System;

public class GetApplicationHandlerTests
{
    [Fact]
    public async Task Handle_ValidApplicationId_ReturnsApplication()
    {
        // Arrange
        var applicationId = Guid.NewGuid();
        var applicationMock = new Application();
        var applicationRepositoryMock = new Mock<IApplicationRepository>();
        applicationRepositoryMock.Setup(repo => repo.GetById(applicationId))
            .ReturnsAsync(applicationMock);

        var handler = new GetApplicationHandler(applicationRepositoryMock.Object);

        // Act
        var result = await handler.Handle(new GetApplication(applicationId), CancellationToken.None);

        // Assert
        Assert.Equal(applicationMock, result);
    }

    [Fact]
    public async Task Handle_InvalidApplicationId_ReturnsNull()
    {
        // Arrange
        var applicationId = Guid.NewGuid();
        var applicationRepositoryMock = new Mock<IApplicationRepository>();
        applicationRepositoryMock.Setup(repo => repo.GetById(applicationId))
            .ReturnsAsync((Application)null);

        var handler = new GetApplicationHandler(applicationRepositoryMock.Object);

        // Act
        var result = await handler.Handle(new GetApplication(applicationId), CancellationToken.None);

        // Assert
        Assert.Null(result);
    }
}