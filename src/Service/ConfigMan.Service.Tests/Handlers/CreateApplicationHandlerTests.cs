using ConfigMan.Data.Handlers.Applications;
using ConfigMan.Data.Models;
using Moq;

namespace ConfigMan.Data.Tests.Handlers.Applications;

public class CreateApplicationHandlerTests
{
    [Fact]
    public async Task Handle_EnvironmentNotFound_ThrowsException()
    {
        // Arrange
        var applicationId = Guid.NewGuid();
        var environmentSetId = Guid.NewGuid();
        var command = new CreateApplication(applicationId, "TestApp", "TestToken", environmentSetId);

        var applicationSessionMock = new Mock<IDocumentSessionHelper<Application>>();
        var environmentSetSessionMock = new Mock<IDocumentSessionHelper<EnvironmentSet>>();
        var userInfoMock = new Mock<IUserInfo>();

        environmentSetSessionMock.Setup(e => e.GetFromEventStream(environmentSetId)).ReturnsAsync((EnvironmentSet)null); // Simulate environment not found

        var handler = new CreateApplicationHandler(applicationSessionMock.Object, environmentSetSessionMock.Object, userInfoMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<NullReferenceException>(async () =>
        {
            await handler.Handle(command, CancellationToken.None);
        });

        // Verify
        environmentSetSessionMock.Verify(e => e.GetFromEventStream(environmentSetId), Times.Once);
        applicationSessionMock.Verify(a => a.Start(applicationId, It.IsAny<ApplicationCreated>(), It.IsAny<Guid>()), Times.Never);
        applicationSessionMock.Verify(a => a.AppendToStream<Application>(command.ApplicationId, It.IsAny<ApplicationEnvironmentAdded>(), It.IsAny<Guid>()), Times.Never);
        environmentSetSessionMock.Verify(e => e.AppendToStream<EnvironmentSet>(command.EnvironmentSetId, It.IsAny<ApplicationAssociatedToEnvironmentSet>(), It.IsAny<Guid>()), Times.Never);
        applicationSessionMock.Verify(a => a.SaveChangesAsync(), Times.Never);
        environmentSetSessionMock.Verify(e => e.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_EnvironmentFound_CreatesApplicationAndAssociatesWithEnvironments()
    {
        // Arrange
        var applicationId = Guid.NewGuid();
        var environmentSetId = Guid.NewGuid();
        var command = new CreateApplication(applicationId, "TestApp", "TestToken", environmentSetId);

        var applicationSessionMock = new Mock<IDocumentSessionHelper<Application>>();
        var environmentSetSessionMock = new Mock<IDocumentSessionHelper<EnvironmentSet>>();
        var userInfoMock = new Mock<IUserInfo>();

        var environmentSet = new EnvironmentSet
        {
            DeploymentEnvironments = new List<DeploymentEnvironment>
            {
                new() { Name = "Env1" },
                new() { Name = "Env2" }
            }
        };
        environmentSetSessionMock.Setup(e => e.GetFromEventStream(environmentSetId)).ReturnsAsync(environmentSet);

        var handler = new CreateApplicationHandler(applicationSessionMock.Object, environmentSetSessionMock.Object, userInfoMock.Object);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Verify
        environmentSetSessionMock.Verify(e => e.GetFromEventStream(environmentSetId), Times.Once);
        applicationSessionMock.Verify(a => a.Start(applicationId, It.Is<ApplicationCreated>(ac => ac.Id == command.ApplicationId && ac.Name == command.Name && ac.Token == command.Token && ac.EnvironmentSetId == command.EnvironmentSetId), It.IsAny<Guid>()), Times.Once);
        applicationSessionMock.Verify(a => a.AppendToStream<Application>(command.ApplicationId, It.IsAny<ApplicationEnvironmentAdded>(), It.IsAny<Guid>()), Times.Exactly(environmentSet.DeploymentEnvironments.Count));
        environmentSetSessionMock.Verify(e => e.AppendToStream<EnvironmentSet>(command.EnvironmentSetId, It.IsAny<ApplicationAssociatedToEnvironmentSet>(), It.IsAny<Guid>()), Times.Exactly(environmentSet.DeploymentEnvironments.Count));
        applicationSessionMock.Verify(a => a.SaveChangesAsync(), Times.Once);
        environmentSetSessionMock.Verify(e => e.SaveChangesAsync(), Times.Once);
    }
}