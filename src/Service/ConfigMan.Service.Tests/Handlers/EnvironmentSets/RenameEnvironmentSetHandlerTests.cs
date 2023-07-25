using ConfigMan.Data;
using ConfigMan.Data.Data;
using ConfigMan.Data.Handlers.Applications;
using ConfigMan.Data.Handlers.EnvironmentSets;
using ConfigMan.Data.Models;
using Microsoft.Extensions.Logging;
using Moq;

namespace ConfigMan.Service.Tests.Handlers.EnvironmentSets;

public class RenameEnvironmentSetHandlerTests
{
    private readonly Mock<IDocumentSessionHelper<EnvironmentSet>> _documentSession;
    private readonly Mock<IEnvironmentSetRepository> _environmentSetRepository;
    private readonly RenameEnvironmentSetHandler _subject;

    public RenameEnvironmentSetHandlerTests()
    {
        _documentSession = new Mock<IDocumentSessionHelper<EnvironmentSet>>();
        _environmentSetRepository = new Mock<IEnvironmentSetRepository>();
        _subject = new RenameEnvironmentSetHandler(
            _documentSession.Object,
            _environmentSetRepository.Object,
            new Mock<ILogger<RenameEnvironmentSetHandler>>().Object
        );
    }

    [Fact]
    public async Task Handle_UniqueNewName_SuccessfulRename()
    {
        // Arrange
        var command = new RenameEnvironmentSet(
            Guid.NewGuid(), // Provide a valid EnvironmentSetId
            1, // Provide a valid ExpectedVersion
            "NewEnvironmentSet" // Provide a unique NewName
        );

        _environmentSetRepository.Setup(x => x.GetByName(command.NewName)).Returns((EnvironmentSet)null!); // Return null to simulate a unique NewName

        // Act
        var response = await _subject.Handle(command, CancellationToken.None);

        // Assert
        Assert.Empty(response.Errors); // Check that there are no errors in the response
        Assert.True(response.WasSuccessful); // Check that the response indicates success
        Assert.Equal(2, response.NewVersion); // Check the value of the NewVersion property (ExpectedVersion + 1)

        _documentSession.Verify(x => x.AppendToStream(command.EnvironmentSetId, command.ExpectedVersion, It.IsAny<EnvironmentSetRenamed>()), Times.Once); // Check that AppendToStream is called once with the correct parameters
        _documentSession.Verify(x => x.SaveChangesAsync(), Times.Once); // Check that SaveChangesAsync is called once for EnvironmentSet
    }

    [Fact]
    public async Task Handle_DuplicateNewName_FailureResponse()
    {
        // Arrange
        var command = new RenameEnvironmentSet(
            Guid.NewGuid(), // Provide a valid EnvironmentSetId
            1, // Provide a valid ExpectedVersion
            "DuplicateEnvironmentSet" // Provide a duplicate NewName
        );

        var existingEnvironmentSet = new EnvironmentSet
        {
            Id = Guid.NewGuid(),
            Name = "DuplicateEnvironmentSet"
        };

        _environmentSetRepository.Setup(x => x.GetByName(command.NewName)).Returns(existingEnvironmentSet); // Return an existing EnvironmentSet to simulate a duplicate NewName

        // Act
        var response = await _subject.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotEmpty(response.Errors); // Check that there are errors in the response
        Assert.Contains(response.Errors, e => e.ErrorCode == Errors.DuplicateName(command.NewName).ErrorCode); // Check that the error message matches the expected error message for duplicate name
    }
}