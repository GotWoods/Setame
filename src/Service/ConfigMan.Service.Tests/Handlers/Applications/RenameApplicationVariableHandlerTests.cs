using ConfigMan.Data;
using ConfigMan.Data.Data;
using ConfigMan.Data.Handlers.Applications;
using ConfigMan.Data.Models;
using Moq;
using Environment = ConfigMan.Data.Models.Environment;

namespace ConfigMan.Service.Tests.Handlers.Applications;

public class RenameApplicationVariableHandlerTests
{
    private readonly Mock<IDocumentSessionHelper<Application>> _documentSession;
    private readonly Mock<IApplicationRepository> _querySession;
    private readonly RenameApplicationVariableHandler _subject;

    public RenameApplicationVariableHandlerTests()
    {
        _documentSession = new Mock<IDocumentSessionHelper<Application>>();
        _querySession = new Mock<IApplicationRepository>();
        _subject = new RenameApplicationVariableHandler(_documentSession.Object, _querySession.Object);
    }

    [Fact]
    public async Task Handle_ValidData_SuccessfulRename()
    {
        // Arrange
        var command = new RenameApplicationVariable(
            Guid.NewGuid(), // Provide a valid ApplicationId
            1, // Provide the expected version
            "Old Variable Name", // Provide the old variable name
            "New Variable Name" // Provide the new variable name
        );

        var existingApplication = new Application
        {
            Id = command.ApplicationId,
            EnvironmentSettings = new List<Environment>
            {
                new Environment
                {
                    Settings = new List<Setting>
                    {
                        new Setting { Name = "Old Variable Name" } // Add the old variable name to an environment setting
                    }
                }
            },
            Version = command.ExpectedVersion - 1 // The expected version should be one less than the current version
        };

        _querySession.Setup(x => x.GetById(command.ApplicationId)).ReturnsAsync(existingApplication);

        // Act
        var response = await _subject.Handle(command, CancellationToken.None);

        // Assert
        Assert.Empty(response.Errors); // Check that there are no errors in the response
        Assert.True(response.WasSuccessful); // Check that the response indicates success
        Assert.Equal(command.ExpectedVersion + 1, response.NewVersion); // Check the value of the NewVersion property
        _documentSession.Verify(x => x.AppendToStream(command.ApplicationId, command.ExpectedVersion, It.IsAny<ApplicationVariableRenamed>()), Times.Once); // Check that AppendToStream is called once with the correct parameters
        _documentSession.Verify(x => x.SaveChangesAsync(), Times.Once); // Check that SaveChangesAsync is called once
    }

    [Fact]
    public async Task Handle_ApplicationNotFound_FailureResponse()
    {
        // Arrange
        var command = new RenameApplicationVariable(
            Guid.NewGuid(), // Provide a valid ApplicationId
            1, // Provide the expected version
            "Old Variable Name",
            "New Variable Name"
        );

        _querySession.Setup(x => x.GetById(command.ApplicationId)).ReturnsAsync((Application)null!); // Return null to simulate that the application is not found

        // Act
        var response = await _subject.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotEmpty(response.Errors); // Check that there are errors in the response
        Assert.Contains(response.Errors, e => e.ErrorCode == Errors.ApplicationNotFound(command.ApplicationId).ErrorCode); // Check that the error message matches the expected error message for application not found
    }

    [Fact]
    public async Task Handle_VariableNotFound_FailureResponse()
    {
        // Arrange
        var command = new RenameApplicationVariable(
            Guid.NewGuid(), // Provide a valid ApplicationId
            1, // Provide the expected version
            "Old Variable Name",
            "New Variable Name"
        );

        var existingApplication = new Application
        {
            Id = command.ApplicationId,
            EnvironmentSettings = new List<Environment>
            {
                new()
                {
                    Settings = new List<Setting>
                    {
                        new Setting { Name = "Some Other Variable" } // Add a different variable name to an environment setting
                    }
                }
            },
            Version = command.ExpectedVersion - 1 // The expected version should be one less than the current version
        };

        _querySession.Setup(x => x.GetById(command.ApplicationId)).ReturnsAsync(existingApplication);

        // Act
        var response = await _subject.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotEmpty(response.Errors); // Check that there are errors in the response
        Assert.Contains(response.Errors, e => e.ErrorCode == Errors.VariableNotFoundRename(command.OldName).ErrorCode); // Check that the error message matches the expected error message for variable not found
    }
}