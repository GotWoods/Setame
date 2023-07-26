using Microsoft.Extensions.Logging;
using Moq;
using Setame.Data;
using Setame.Data.Data;
using Setame.Data.Handlers.EnvironmentSets;
using Setame.Data.Models;
using Setame.Data.Projections;

namespace Setame.Tests.Handlers.EnvironmentSets;

public class DeleteEnvironmentFromEnvironmentSetHandlerTests
{
    private readonly Mock<IDocumentSessionHelper<Application>> _applicationDocumentSession;
    private readonly Mock<IDocumentSessionHelper<EnvironmentSet>> _documentSession;
    private readonly Mock<IEnvironmentSetApplicationAssociationRepository> _environmentSetApplicationAssociationRepository;
    private readonly DeleteEnvironmentFromEnvironmentSetHandler _subject;

    public DeleteEnvironmentFromEnvironmentSetHandlerTests()
    {
        _documentSession = new Mock<IDocumentSessionHelper<EnvironmentSet>>();
        _applicationDocumentSession = new Mock<IDocumentSessionHelper<Application>>();
        _environmentSetApplicationAssociationRepository = new Mock<IEnvironmentSetApplicationAssociationRepository>();
        _subject = new DeleteEnvironmentFromEnvironmentSetHandler(
            _documentSession.Object,
            _applicationDocumentSession.Object,
            _environmentSetApplicationAssociationRepository.Object,
            new Mock<ILogger<DeleteEnvironmentFromEnvironmentSetHandler>>().Object
        );
    }

    [Fact]
    public async Task Handle_DeleteEnvironment_Successful()
    {
        // Arrange
        var command = new DeleteEnvironmentFromEnvironmentSet(
            Guid.NewGuid(), // Provide a valid EnvironmentSetId
            1, // Provide a valid ExpectedVersion
            "Dev" // Provide the name of the environment to be deleted
        );

        var associations = new EnvironmentSetApplicationAssociation
        {
            Id = command.EnvironmentSetId,
            Applications = new List<SimpleApplication>() { new SimpleApplication(Guid.NewGuid(), "one"), new SimpleApplication(Guid.NewGuid(), "two") }
        };

        _environmentSetApplicationAssociationRepository.Setup(x => x.Get(command.EnvironmentSetId)).Returns(associations);

        // Act
        var response = await _subject.Handle(command, CancellationToken.None);

        // Assert
        Assert.Empty(response.Errors); // Check that there are no errors in the response
        Assert.True(response.WasSuccessful); // Check that the response indicates success
        Assert.Equal(2, response.NewVersion); // Check the value of the NewVersion property (ExpectedVersion + 1)

        _documentSession.Verify(x => x.AppendToStream(command.EnvironmentSetId, command.ExpectedVersion, It.IsAny<EnvironmentRemoved>()), Times.Once); // Check that AppendToStream is called once with the correct parameters for EnvironmentSet
        _documentSession.Verify(x => x.SaveChangesAsync(), Times.Once); // Check that SaveChangesAsync is called once for EnvironmentSet

        foreach (var application in associations.Applications)
        {
            _applicationDocumentSession.Verify(x => x.AppendToStream(application.Id, It.IsAny<EnvironmentRemoved>()), Times.Once); // Check that AppendToStream is called once with the correct parameters for each application
            _applicationDocumentSession.Verify(x => x.SaveChangesAsync(), Times.Once); // Check that SaveChangesAsync is called once for each application
        }
    }
}