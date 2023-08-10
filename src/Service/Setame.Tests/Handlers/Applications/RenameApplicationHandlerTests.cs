using Microsoft.Extensions.Logging;
using Moq;
using Setame.Data;
using Setame.Data.Data;
using Setame.Data.Handlers.Applications;
using Setame.Data.Models;
using Setame.Data.Projections;
using Xunit;

namespace Setame.Tests.Handlers.Applications;

public class RenameApplicationHandlerTests
{
    private readonly Mock<IDocumentSessionHelper<Application>> _documentSession;
    private readonly Mock<IApplicationRepository> _querySession;
    private readonly Mock<ILogger<RenameApplicationHandler>> _loggerMock;
    private readonly RenameApplicationHandler _subject;

    public RenameApplicationHandlerTests()
    {
        _documentSession = new Mock<IDocumentSessionHelper<Application>>();
        _querySession = new Mock<IApplicationRepository>();
        _loggerMock = new Mock<ILogger<RenameApplicationHandler>>();
        _subject = new RenameApplicationHandler(_documentSession.Object, _querySession.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_DuplicateName_WarnsAndReturnsError()
    {
        // Arrange
        var command = new RenameApplication(Guid.NewGuid(), 1, "New Application Name");

        var existingApplication = new ActiveApplication(); // Just some mock application data

        _querySession.Setup(x => x.GetByName(command.NewName)).Returns(existingApplication);

        // Act
        var response = await _subject.Handle(command, CancellationToken.None);

        // Assert
        Assert.Single(response.Errors);
        Assert.Contains(response.Errors, e => e.ErrorCode == Errors.DuplicateName(command.NewName).ErrorCode);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString() == $"Could not rename to {command.NewName} as an application already has that name"),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
    }

    [Fact]
    public async Task Handle_UniqueName_RenamesSuccessfully()
    {
        // Arrange
        var command = new RenameApplication(Guid.NewGuid(), 1, "Unique Application Name");

        _querySession.Setup(x => x.GetByName(command.NewName)).Returns((ActiveApplication)null);

        // Act
        var response = await _subject.Handle(command, CancellationToken.None);

        // Assert
        Assert.Empty(response.Errors);
        _documentSession.Verify(x => x.AppendToStream(command.ApplicationId, command.ExpectedVersion, It.IsAny<ApplicationRenamed>()), Times.Once);
        _documentSession.Verify(x => x.SaveChangesAsync(), Times.Once);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Debug,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString() == $"Renaming {command.ApplicationId} to {command.NewName}"),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Debug,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString() == "Application renamed"),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
    }
}
