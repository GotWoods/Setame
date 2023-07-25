using ConfigMan.Data;
using ConfigMan.Data.Data;
using ConfigMan.Data.Handlers.Applications;
using ConfigMan.Data.Models;
using Microsoft.Extensions.Logging;
using Moq;
using Environment = ConfigMan.Data.Models.Environment;

namespace ConfigMan.Service.Tests.Handlers.Applications;

public class UpdateApplicationVariableHandlerTests
{
    private readonly Mock<IDocumentSessionHelper<Application>> _documentSession;
    private readonly Mock<IApplicationRepository> _querySession;
    private readonly UpdateApplicationVariableHandler _subject;

    public UpdateApplicationVariableHandlerTests()
    {
        _documentSession = new Mock<IDocumentSessionHelper<Application>>();
        _querySession = new Mock<IApplicationRepository>();
        _subject = new UpdateApplicationVariableHandler(_documentSession.Object, _querySession.Object, new Mock<ILogger<UpdateApplicationVariableHandler>>().Object);
    }

    [Fact]
    public async Task Handle_ValidData_SuccessfulUpdate()
    {
        // Arrange
        var command = new UpdateApplicationVariable(
            Guid.NewGuid(), // Provide a valid ApplicationId
            1, // Provide the expected version
            "Environment Name", // Provide an existing environment name
            "Variable Name", // Provide an existing variable name
            "New Value" // Provide the new value for the variable
        );

        var existingApplication = new Application
        {
            Id = command.ApplicationId,
            EnvironmentSettings = new List<Environment>
            {
                new Environment
                {
                    Name = "Environment Name",
                    Settings = new List<Setting>
                    {
                        new Setting { Name = "Variable Name", Value = "Old Value" } // Add the existing variable to the existing environment setting
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
        _documentSession.Verify(x => x.AppendToStream(command.ApplicationId, command.ExpectedVersion, It.IsAny<ApplicationVariableChanged>()), Times.Once); // Check that AppendToStream is called once with the correct parameters
        _documentSession.Verify(x => x.SaveChangesAsync(), Times.Once); // Check that SaveChangesAsync is called once
    }

    [Fact]
    public async Task Handle_ApplicationNotFound_FailureResponse()
    {
        // Arrange
        var command = new UpdateApplicationVariable(
            Guid.NewGuid(), // Provide a valid ApplicationId
            1, // Provide the expected version
            "Environment Name",
            "Variable Name",
            "New Value"
        );

        _querySession.Setup(x => x.GetById(command.ApplicationId)).ReturnsAsync((Application)null!); // Return null to simulate that the application is not found

        // Act
        var response = await _subject.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotEmpty(response.Errors); // Check that there are errors in the response
        Assert.Contains(response.Errors, e => e.ErrorCode == Errors.ApplicationNotFound(command.ApplicationId).ErrorCode); // Check that the error message matches the expected error message for application not found
    }

    [Fact]
    public async Task Handle_EnvironmentNotFound_FailureResponse()
    {
        // Arrange
        var command = new UpdateApplicationVariable(
            Guid.NewGuid(), // Provide a valid ApplicationId
            1, // Provide the expected version
            "Environment Name", // Provide a non-existing environment name
            "Variable Name",
            "New Value"
        );

        var existingApplication = new Application
        {
            Id = command.ApplicationId,
            EnvironmentSettings = new List<Environment>
            {
                new Environment
                {
                    Name = "Some Other Environment",
                    Settings = new List<Setting>
                    {
                        new Setting { Name = "Variable Name", Value = "Old Value" }
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
        Assert.Contains(response.Errors, e => e.ErrorCode == Errors.EnvironmentNotFound(command.Environment).ErrorCode); // Check that the error message matches the expected error message for environment not found
    }

    [Fact]
    public async Task Handle_VariableNotFound_FailureResponse()
    {
        // Arrange
        var command = new UpdateApplicationVariable(
            Guid.NewGuid(), // Provide a valid ApplicationId
            1, // Provide the expected version
            "Environment Name",
            "Variable Name", // Provide a non-existing variable name
            "New Value"
        );

        var existingApplication = new Application
        {
            Id = command.ApplicationId,
            EnvironmentSettings = new List<Environment>
            {
                new Environment
                {
                    Name = "Environment Name",
                    Settings = new List<Setting>
                    {
                        new Setting { Name = "Some Other Variable", Value = "Old Value" } // Add a different variable to the existing environment setting
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
        Assert.Contains(response.Errors, e => e.ErrorCode == Errors.VariableNotFound(command.VariableName).ErrorCode); // Check that the error message matches the expected error message for variable not found
    }
}