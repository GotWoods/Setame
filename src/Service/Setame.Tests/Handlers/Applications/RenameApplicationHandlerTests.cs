using Microsoft.Extensions.Logging;
using Moq;
using Setame.Data;
using Setame.Data.Data;
using Setame.Data.Handlers.Applications;
using Setame.Data.Models;
using Setame.Data.Projections;

namespace Setame.Tests.Handlers.Applications;

public class RenameApplicationHandlerTests
{
    private readonly Mock<IDocumentSessionHelper<Application>> _documentSession;
    private readonly Mock<IApplicationRepository> _querySession;
    private readonly RenameApplicationHandler _subject;

    public RenameApplicationHandlerTests()
    {
        _documentSession = new Mock<IDocumentSessionHelper<Application>>();
        _querySession = new Mock<IApplicationRepository>();
        _subject = new RenameApplicationHandler(_documentSession.Object, _querySession.Object, new Mock<ILogger<RenameApplicationHandler>>().Object);
    }

    [Fact]
    public async Task Handle_ValidData_SuccessfulRename()
    {
        // Arrange
        var command = new RenameApplication(Guid.NewGuid(), 1, "New Application Name"); // Provide a valid ApplicationId and ExpectedVersion

        var existingApplication = new ActiveApplication()
        {
            Id = command.ApplicationId,
            Name = "Old Application Name",
            Version = command.ExpectedVersion - 1 // The expected version should be one less than the current version
        };

        _querySession.Setup(x => x.GetByName(command.NewName)).Returns(existingApplication);

        // Act
        var response = await _subject.Handle(command, CancellationToken.None);

        // Assert
        Assert.Empty(response.Errors); // Check that there are no errors in the response
        Assert.True(response.WasSuccessful); // Check that the response indicates success
        _documentSession.Verify(x => x.AppendToStream(command.ApplicationId, command.ExpectedVersion, It.IsAny<ApplicationRenamed>()), Times.Once); // Check that AppendToStream is called once with the correct parameters
        _documentSession.Verify(x => x.SaveChangesAsync(), Times.Once); // Check that SaveChangesAsync is called once
    }

    [Fact]
    public async Task Handle_ApplicationNotFound_FailureResponse()
    {
        // Arrange
        var command = new RenameApplication(Guid.NewGuid(), 1, "New Application Name"); // Provide a valid ApplicationId and ExpectedVersion

        _querySession.Setup(x => x.GetById(command.ApplicationId)).ReturnsAsync((Application)null!); // Return null to simulate that the application is not found

        // Act
        var response = await _subject.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotEmpty(response.Errors); // Check that there are errors in the response
        Assert.Contains(response.Errors, e => e.ErrorCode == Errors.ApplicationNotFound(command.ApplicationId).ErrorCode); // Check that the error message matches the expected error message for application not found
    }
}