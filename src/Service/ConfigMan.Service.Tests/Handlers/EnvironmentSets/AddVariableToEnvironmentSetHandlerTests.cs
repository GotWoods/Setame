using ConfigMan.Data;
using ConfigMan.Data.Data;
using ConfigMan.Data.Handlers.EnvironmentSets;
using ConfigMan.Data.Models;
using Moq;

namespace ConfigMan.Service.Tests.Handlers.EnvironmentSets;

public class AddVariableToEnvironmentSetHandlerTests
{
    private readonly Mock<IDocumentSessionHelper<EnvironmentSet>> _documentSession;
    private readonly Mock<IEnvironmentSetRepository> _environmentSetRepository;
    private readonly AddVariableToEnvironmentSetHandler _subject;

    public AddVariableToEnvironmentSetHandlerTests()
    {
        _documentSession = new Mock<IDocumentSessionHelper<EnvironmentSet>>();
        _environmentSetRepository = new Mock<IEnvironmentSetRepository>();
        _subject = new AddVariableToEnvironmentSetHandler(_documentSession.Object, _environmentSetRepository.Object);
    }

    [Fact]
    public async Task Handle_ValidData_SuccessfulAddition()
    {
        // Arrange
        var command = new AddVariableToEnvironmentSet(
            Guid.NewGuid(), // Provide a valid EnvironmentSetId
            1, // Provide the expected version
            "Variable Name2" // Provide a variable name that does not exist in one of the deployment environments
        );

        var existingEnvironmentSet = new EnvironmentSet
        {
            Id = command.EnvironmentSetId,
            DeploymentEnvironments = new List<DeploymentEnvironment>
            {
                new DeploymentEnvironment
                {
                    Name = "Dev",
                    EnvironmentSettings = new Dictionary<string, string>
                    {
                        { "Variable Name", "Value" } // Add the variable to one of the deployment environments
                    }
                }
            },
            Version = command.ExpectedVersion - 1 // The expected version should be one less than the current version
        };

        _environmentSetRepository.Setup(x => x.GetById(command.EnvironmentSetId)).ReturnsAsync(existingEnvironmentSet);

        // Act
        var response = await _subject.Handle(command, CancellationToken.None);

        // Assert
        Assert.Empty(response.Errors); // Check that there are no errors in the response
        Assert.True(response.WasSuccessful); // Check that the response indicates success
        Assert.Equal(command.ExpectedVersion + 1, response.NewVersion); // Check the value of the NewVersion property
        _documentSession.Verify(x => x.AppendToStream(command.EnvironmentSetId, command.ExpectedVersion, It.IsAny<EnvironmentSetVariableAdded>()), Times.Once); // Check that AppendToStream is called once with the correct parameters
        _documentSession.Verify(x => x.SaveChangesAsync(), Times.Once); // Check that SaveChangesAsync is called once
    }

    [Fact]
    public async Task Handle_EnvironmentSetNotFound_FailureResponse()
    {
        // Arrange
        var command = new AddVariableToEnvironmentSet(
            Guid.NewGuid(), // Provide a valid EnvironmentSetId
            1, // Provide the expected version
            "Variable Name"
        );

        _environmentSetRepository.Setup(x => x.GetById(command.EnvironmentSetId)).ReturnsAsync((EnvironmentSet)null!); // Return null to simulate that the environment set is not found

        // Act
        await Assert.ThrowsAsync<NullReferenceException>(async () => await _subject.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_VariableNotFound_FailureResponse()
    {
        // Arrange
        var command = new AddVariableToEnvironmentSet(
            Guid.NewGuid(), // Provide a valid EnvironmentSetId
            1, // Provide the expected version
            "Variable Name" // Provide a variable name that does not exist in any of the deployment environments
        );

        var existingEnvironmentSet = new EnvironmentSet
        {
            Id = command.EnvironmentSetId,
            DeploymentEnvironments = new List<DeploymentEnvironment>
            {
                new DeploymentEnvironment
                {
                    Name = "Dev",
                    EnvironmentSettings = new Dictionary<string, string>
                    {
                        { "Some Other Variable", "Value" } // Add a different variable to the deployment environment
                    }
                }
            },
            Version = command.ExpectedVersion - 1 // The expected version should be one less than the current version
        };

        _environmentSetRepository.Setup(x => x.GetById(command.EnvironmentSetId)).ReturnsAsync(existingEnvironmentSet);

        // Act
        var response = await _subject.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotEmpty(response.Errors); // Check that there are errors in the response
        Assert.Contains(response.Errors, e => e.ErrorCode == Errors.VariableNotFound(command.VariableName).ErrorCode); // Check that the error message matches the expected error message for variable not found
    }

    [Fact]
    public async Task Handle_DuplicateVariable_FailureResponse()
    {
        // Arrange
        var command = new AddVariableToEnvironmentSet(
            Guid.NewGuid(), // Provide a valid EnvironmentSetId
            1, // Provide the expected version
            "Variable Name" // Provide a variable name that already exists in one of the deployment environments
        );

        var existingEnvironmentSet = new EnvironmentSet
        {
            Id = command.EnvironmentSetId,
            DeploymentEnvironments = new List<DeploymentEnvironment>
            {
                new DeploymentEnvironment
                {
                    Name = "Dev",
                    EnvironmentSettings = new Dictionary<string, string>
                    {
                        { "Variable Name", "Value" } // Add the variable with the same name to the deployment environment
                    }
                }
            },
            Version = command.ExpectedVersion - 1 // The expected version should be one less than the current version
        };

        _environmentSetRepository.Setup(x => x.GetById(command.EnvironmentSetId)).ReturnsAsync(existingEnvironmentSet);

        // Act
        var response = await _subject.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotEmpty(response.Errors); // Check that there are errors in the response
        Assert.Contains(response.Errors, e => e.ErrorCode == Errors.DuplicateName(command.VariableName).ErrorCode); // Check that the error message matches the expected error message for duplicate variable name
    }
}