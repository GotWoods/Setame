using Microsoft.Extensions.Logging;
using Moq;
using Setame.Data;
using Setame.Data.Data;
using Setame.Data.Handlers.Applications;
using Setame.Data.Models;

namespace Setame.Tests.Handlers.Applications;

public class CreateDefaultApplicationVariableHandlerTests
{
    private readonly Mock<IDocumentSessionHelper<Application>> _documentSession;
    private readonly Mock<IApplicationRepository> _applicationRepository;
    private readonly CreateDefaultApplicationVariableHandler _subject;

    public CreateDefaultApplicationVariableHandlerTests()
    {
        _documentSession = new Mock<IDocumentSessionHelper<Application>>();
        _applicationRepository = new Mock<IApplicationRepository>();
        _subject = new CreateDefaultApplicationVariableHandler(_documentSession.Object, _applicationRepository.Object, new Mock<ILogger<CreateDefaultApplicationVariableHandler>>().Object);
    }

    [Fact]
    public async Task Handle_ValidData_SuccessfulResponse()
    {
        // Arrange
        var command = new CreateDefaultApplicationVariable(
            Guid.NewGuid(), // Provide a valid ApplicationId
            1, // Provide the expected version of the application
            "VariableName" // Provide a unique variable name
        );

        var application = new Application
        {
            Id = command.ApplicationId,
            ApplicationDefaults = new List<Setting>
            {
                new Setting
                {
                    Name = "ExistingVariable" // Add an existing variable to the application defaults
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
    }

   

    [Fact]
    public async Task Handle_DuplicateVariableName_FailureResponse()
    {
        // Arrange
        var command = new CreateDefaultApplicationVariable(
            Guid.NewGuid(),
            1,
            "VariableName"
        );

        var application = new Application
        {
            Id = command.ApplicationId,
            ApplicationDefaults = new List<Setting>
            {
                new Setting
                {
                    Name = command.VariableName // Add the same variable name to the application defaults
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