using Moq;
using Setame.Data.Data;
using Setame.Data.Handlers.EnvironmentSets;
using Setame.Data.Models;
using Setame.Data.Projections;

namespace Setame.Tests.Handlers.EnvironmentSets;

public class GetActiveEnvironmentSetsHandlerTests
{
    [Fact]
    public async Task Handle_RepositoryReturnsActiveEnvironmentSets_ReturnsListOfEnvironmentSets()
    {
        // Arrange
        var firstId = Guid.NewGuid();
        var secondId = Guid.NewGuid();
        var activeEnvironmentSetsMock = new List<ActiveEnvironmentSet>
        {
            new ActiveEnvironmentSet { Id = firstId, },  
            new ActiveEnvironmentSet { Id = secondId, }
        };

        var environmentSetRepositoryMock = new Mock<IEnvironmentSetRepository>();
        environmentSetRepositoryMock.Setup(repo => repo.GetAllActiveEnvironmentSets())
            .ReturnsAsync(activeEnvironmentSetsMock);

        environmentSetRepositoryMock.Setup(repo => repo.GetById(It.IsAny<Guid>())).ReturnsAsync((Guid id) => new EnvironmentSet { Id = id, Name = $"Name{id}" });

        var handler = new GetActiveEnvironmentSetsHandler(environmentSetRepositoryMock.Object);

        // Act
        var result = await handler.Handle(new GetActiveEnvironmentSets(), CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("Name" + firstId, result.First().Name);
        Assert.Equal("Name" + secondId, result.Last().Name);
    }

    [Fact]
    public async Task Handle_RepositoryReturnsEmptyList_ReturnsEmptyList()
    {
        // Arrange
        var activeEnvironmentSetsMock = new List<ActiveEnvironmentSet>();
        var environmentSetRepositoryMock = new Mock<IEnvironmentSetRepository>();
        environmentSetRepositoryMock.Setup(repo => repo.GetAllActiveEnvironmentSets())
            .ReturnsAsync(activeEnvironmentSetsMock);

        var handler = new GetActiveEnvironmentSetsHandler(environmentSetRepositoryMock.Object);

        // Act
        var result = await handler.Handle(new GetActiveEnvironmentSets(), CancellationToken.None);

        // Assert
        Assert.Empty(result);
    }
}