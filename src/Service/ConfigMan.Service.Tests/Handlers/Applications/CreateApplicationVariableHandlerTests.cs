using ConfigMan.Data;
using ConfigMan.Data.Data;
using ConfigMan.Data.Handlers.Applications;
using ConfigMan.Data.Models;
using Moq;
using Environment = ConfigMan.Data.Models.Environment;

namespace ConfigMan.Service.Tests.Handlers.Applications;

public class CreateApplicationVariableHandlerTests
{
    private readonly Mock<IDocumentSessionHelper<Application>> _documentSession;
    private readonly Mock<IApplicationRepository> _applicationRepository;
    private readonly CreateApplicationVariableHandler _subject;

    public CreateApplicationVariableHandlerTests()
    {
        _documentSession = new Mock<IDocumentSessionHelper<Application>>();
        _applicationRepository = new Mock<IApplicationRepository>();
        _subject = new CreateApplicationVariableHandler(_documentSession.Object, _applicationRepository.Object);
    }

    [Fact]
    public async Task Handle_ValidData_SuccessfulResponse()
    {
        // Arrange
        var command = new CreateApplicationVariable(
            Guid.NewGuid(), // Provide a valid ApplicationId
            1, // Provide the expected version of the application
            "Dev", // Provide an existing environment
            "VariableName" // Provide a unique variable name
        );

        var application = new Application
        {
            Id = command.ApplicationId,
            Version = command.ExpectedVersion,
            EnvironmentSettings = new List<Environment>
            {
                new Environment()
                {
                    Name = "Dev",
                    Settings = new List<Setting>
                    {
                        new Setting { Name = "ExistingVariable" } // Add an existing variable to the Dev environment
                    }
                }
            }
        };

        _applicationRepository.Setup(x => x.GetById(command.ApplicationId))
            .ReturnsAsync(application);

        // Act
        var response = await _subject.Handle(command, CancellationToken.None);

        // Assert
        Assert.Empty(response.Errors); // Check that there are no errors in the response
        Assert.True(response.WasSuccessful); // Check that the response indicates success
        Assert.Equal(application.Version + 1, response.NewVersion); // Check the value of the NewVersion property
    }

    [Fact]
    public async Task Handle_InvalidApplication_FailureResponse()
    {
        // Arrange
        var command = new CreateApplicationVariable(
            Guid.NewGuid(),
            1,
            "Dev",
            "VariableName"
        );

        _applicationRepository.Setup(x => x.GetById(command.ApplicationId))
            .ReturnsAsync((Application)null!); // Return null to simulate an invalid application

        // Act
        var ex = await Assert.ThrowsAsync<NullReferenceException>(async () => await _subject.Handle(command, CancellationToken.None));

        // Assert
        Assert.Equal("Application could not be found", ex.Message); // Check the exception message
    }

    [Fact]
    public async Task Handle_InvalidEnvironment_FailureResponse()
    {
        // Arrange
        var command = new CreateApplicationVariable(
            Guid.NewGuid(),
            1,
            "Dev",
            "VariableName"
        );

        var application = new Application
        {
            Id = command.ApplicationId,
            Version = command.ExpectedVersion,
            EnvironmentSettings = new List<Environment>
            {
                new Environment
                {
                    Name = "Prod" // Add a different environment (not "Dev")
                }
            }
        };

        _applicationRepository.Setup(x => x.GetById(command.ApplicationId))
            .ReturnsAsync(application);

        // Act
        var ex = await Assert.ThrowsAsync<NullReferenceException>(async () => await _subject.Handle(command, CancellationToken.None));

        // Assert
        Assert.Equal("Environment " + command.Environment + " could not be found on the application", ex.Message); // Check the exception message
    }

    [Fact]
    public async Task Handle_DuplicateVariableName_FailureResponse()
    {
        // Arrange
        var command = new CreateApplicationVariable(
            Guid.NewGuid(),
            1,
            "Dev",
            "VariableName"
        );

        var application = new Application
        {
            Id = command.ApplicationId,
            Version = command.ExpectedVersion,
            EnvironmentSettings = new List<Environment>
            {
                new Environment
                {
                    Name = "Dev",
                    Settings = new List<Setting>
                    {
                        new Setting { Name = command.VariableName } // Add the same variable name to the Dev environment
                    }
                }
            }
        };

        _applicationRepository.Setup(x => x.GetById(command.ApplicationId))
            .ReturnsAsync(application);

        // Act
        var response = await _subject.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(response.WasSuccessful); // Check that the response indicates failure
        Assert.NotEmpty(response.Errors); // Check that there are errors in the response
        Assert.Single(response.Errors); // Check that there is only one error
        Assert.Contains(response.Errors, e => e.ErrorCode == Errors.DuplicateName(command.VariableName).ErrorCode); // Check that the error message matches the expected error message for a duplicate variable name
    }
}