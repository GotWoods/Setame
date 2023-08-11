using Xunit;
using Moq;
using MediatR;
using Setame.Data.Projections;
using Setame.Data.Handlers.Applications;
using Setame.Data.Data;
using System.Collections.Generic;

public class GetActiveApplicationsHandlerTests
{
    [Fact]
    public async Task Handle_RepositoryReturnsApplications_ReturnsListOfApplications()
    {
        // Arrange
        var activeApplicationsMock = new List<ActiveApplication>
        {
            new ActiveApplication(),
            new ActiveApplication()
        };
        var applicationRepositoryMock = new Mock<IApplicationRepository>();
        applicationRepositoryMock.Setup(repo => repo.GetAllActive())
            .Returns(activeApplicationsMock);

        var handler = new GetActiveApplicationsHandler(applicationRepositoryMock.Object);

        // Act
        var result = await handler.Handle(new GetActiveApplications(), CancellationToken.None);

        // Assert
        Assert.Equal(activeApplicationsMock, result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task Handle_RepositoryReturnsEmptyList_ReturnsEmptyList()
    {
        // Arrange
        var activeApplicationsMock = new List<ActiveApplication>();
        var applicationRepositoryMock = new Mock<IApplicationRepository>();
        applicationRepositoryMock.Setup(repo => repo.GetAllActive())
            .Returns(activeApplicationsMock);

        var handler = new GetActiveApplicationsHandler(applicationRepositoryMock.Object);

        // Act
        var result = await handler.Handle(new GetActiveApplications(), CancellationToken.None);

        // Assert
        Assert.Empty(result);
    }
}