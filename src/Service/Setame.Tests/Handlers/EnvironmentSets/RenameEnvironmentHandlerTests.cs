using Microsoft.Extensions.Logging;
using Moq;
using Setame.Data;
using Setame.Data.Data;
using Setame.Data.Handlers.EnvironmentSets;
using Setame.Data.Models;
using Setame.Data.Projections;

namespace Setame.Tests.Handlers.EnvironmentSets;

public class RenameEnvironmentHandlerTests
{
    private readonly Mock<IDocumentSessionHelper<Application>> _applicationSession;
    private readonly Mock<IDocumentSessionHelper<EnvironmentSet>> _documentSession;
    private readonly Mock<IEnvironmentSetApplicationAssociationRepository> _environmentSetApplicationAssociationRepository;
    private readonly Mock<IEnvironmentSetRepository> _environmentSetRepository;
    private readonly RenameEnvironmentHandler _subject;

    public RenameEnvironmentHandlerTests()
    {
        _documentSession = new Mock<IDocumentSessionHelper<EnvironmentSet>>();
        _applicationSession = new Mock<IDocumentSessionHelper<Application>>();
        _environmentSetApplicationAssociationRepository = new Mock<IEnvironmentSetApplicationAssociationRepository>();
        _environmentSetRepository = new Mock<IEnvironmentSetRepository>();
        _subject = new RenameEnvironmentHandler(
            _documentSession.Object,
            _applicationSession.Object,
            _environmentSetRepository.Object,
            _environmentSetApplicationAssociationRepository.Object,
            new Mock<ILogger<RenameEnvironmentHandler>>().Object
        );
    }

    [Fact]
    public async Task Handle_RenameEnvironment_Successful()
    {
        // Arrange
        var command = new RenameEnvironment(
            Guid.NewGuid(), // Provide a valid EnvironmentSetId
            1, // Provide a valid ExpectedVersion
            "OldEnvironment", // Provide the old name of the environment
            "NewEnvironment" // Provide the new name for the environment
        );

        var environmentSet = new EnvironmentSet
        {
            Id = command.EnvironmentSetId,
            Environments = new List<DeploymentEnvironment>
            {
                new DeploymentEnvironment {Name = "OldEnvironment"},
                new DeploymentEnvironment {Name = "OtherEnvironment"}
            }
        };

        var associations = new EnvironmentSetApplicationAssociation
        {
            Id = command.EnvironmentSetId,
            Applications = new List<SimpleApplication>() { new SimpleApplication(Guid.NewGuid(), "one"), new SimpleApplication(Guid.NewGuid(), "two") }
        };

        _environmentSetRepository.Setup(x => x.GetById(command.EnvironmentSetId)).ReturnsAsync(environmentSet);
        _environmentSetApplicationAssociationRepository.Setup(x => x.Get(command.EnvironmentSetId)).Returns(associations);

        // Act
        var response = await _subject.Handle(command, CancellationToken.None);

        // Assert
        Assert.Empty(response.Errors); // Check that there are no errors in the response
        Assert.True(response.WasSuccessful); // Check that the response indicates success
        Assert.Equal(2, response.NewVersion); // Check the value of the NewVersion property (ExpectedVersion + 1)

        _documentSession.Verify(x => x.AppendToStream(command.EnvironmentSetId, command.ExpectedVersion, It.IsAny<EnvironmentRenamed>()), Times.Once); // Check that AppendToStream is called once with the correct parameters for EnvironmentSet
        _documentSession.Verify(x => x.SaveChangesAsync(), Times.Once); // Check that SaveChangesAsync is called once for EnvironmentSet

        foreach (var application in associations.Applications)
        {
            _applicationSession.Verify(x => x.AppendToStream(application.Id, It.IsAny<EnvironmentRenamed>()), Times.Once); // Check that AppendToStream is called once with the correct parameters for each application
            
        }
        _applicationSession.Verify(x => x.SaveChangesAsync(), Times.Once); // Check that SaveChangesAsync is called once for each application
    }

    [Fact]
    public async Task Handle_RenameEnvironment_EnvironmentExistsWithNewName_Failure()
    {
        // Arrange
        var command = new RenameEnvironment(
            Guid.NewGuid(), // Provide a valid EnvironmentSetId
            1, // Provide a valid ExpectedVersion
            "OldEnvironment", // Provide the old name of the environment
            "OtherEnvironment" // Provide a new name that already exists in the environment set
        );

        var environmentSet = new EnvironmentSet
        {
            Id = command.EnvironmentSetId,
            Environments = new List<DeploymentEnvironment>
            {
                new DeploymentEnvironment {Name = "OldEnvironment"},
                new DeploymentEnvironment {Name = "OtherEnvironment"}
            }
        };

        _environmentSetRepository.Setup(x => x.GetById(command.EnvironmentSetId)).ReturnsAsync(environmentSet);

        // Act
        var response = await _subject.Handle(command, CancellationToken.None);

        // Assert
        Assert.Single(response.Errors); // Check that there is an error in the response
        Assert.Contains(response.Errors, error => error.ErrorCode == Errors.DuplicateName(command.NewName).ErrorCode); // Check that the error message matches the expected error message for a duplicate name
    }

    [Fact]
    public async Task Handle_RenameEnvironment_EnvironmentSetNotFound_Exception()
    {
        // Arrange
        var command = new RenameEnvironment(
            Guid.NewGuid(), // Provide an invalid EnvironmentSetId
            1, // Provide a valid ExpectedVersion
            "OldEnvironment", // Provide the old name of the environment
            "NewEnvironment" // Provide the new name for the environment
        );

        _environmentSetRepository.Setup(x => x.GetById(command.EnvironmentSetId)).ReturnsAsync((EnvironmentSet)null!); // Return null to simulate environment set not found

        // Act and Assert
        await Assert.ThrowsAsync<NullReferenceException>(async () => await _subject.Handle(command, CancellationToken.None));
    }
}