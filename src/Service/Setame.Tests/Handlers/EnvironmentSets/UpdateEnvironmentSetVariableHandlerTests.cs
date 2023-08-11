using Xunit;
using Moq;
using Setame.Data.Handlers.EnvironmentSets;
using Setame.Data.Models;
using Setame.Data.Data;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Setame.Data;

public class UpdateEnvironmentSetVariableHandlerTests
{
    [Fact]
    public async Task Handle_SuccessfullyUpdatesVariable_ReturnsIncrementedVersion()
    {
        // Arrange
        var command = new UpdateEnvironmentSetVariable(Guid.NewGuid(), 1, "TestEnvironment", "TestVariable", "TestValue");
        var existingEnvironmentSet = new EnvironmentSet
        {
            Environments = new List<DeploymentEnvironment>
            {
                new DeploymentEnvironment
                {
                    Settings = new Dictionary<string, string>
                    {
                        { "TestVariable", "OriginalValue" }
                    }
                }
            }
        };

        var repoMock = new Mock<IEnvironmentSetRepository>();
        repoMock.Setup(repo => repo.GetById(It.IsAny<Guid>())).ReturnsAsync(existingEnvironmentSet);

        var sessionMock = new Mock<IDocumentSessionHelper<EnvironmentSet>>();
        sessionMock.Setup(session => session.AppendToStream(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<object>()));
        sessionMock.Setup(session => session.SaveChangesAsync());

        var loggerMock = new Mock<ILogger<UpdateEnvironmentSetVariableHandler>>();

        var handler = new UpdateEnvironmentSetVariableHandler(sessionMock.Object, repoMock.Object, loggerMock.Object);

        // Act
        var response = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(response.WasSuccessful);
        Assert.Equal(2, response.NewVersion);
    }
}