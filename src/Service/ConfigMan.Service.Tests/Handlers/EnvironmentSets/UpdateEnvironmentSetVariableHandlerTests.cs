using ConfigMan.Data;
using ConfigMan.Data.Handlers.EnvironmentSets;
using ConfigMan.Data.Models;
using Moq;

namespace ConfigMan.Service.Tests.Handlers.EnvironmentSets;

public class UpdateEnvironmentSetVariableHandlerTests
{
    private readonly Mock<IDocumentSessionHelper<EnvironmentSet>> _documentSession;
    private readonly UpdateEnvironmentSetVariableHandler _subject;

    public UpdateEnvironmentSetVariableHandlerTests()
    {
        _documentSession = new Mock<IDocumentSessionHelper<EnvironmentSet>>();
        _subject = new UpdateEnvironmentSetVariableHandler(_documentSession.Object);
    }

    [Fact]
    public async Task Handle_UpdateVariable_SuccessfulUpdate()
    {
        // Arrange
        var command = new UpdateEnvironmentSetVariable(
            Guid.NewGuid(), // Provide a valid EnvironmentSetId
            1, // Provide a valid ExpectedVersion
            "Dev", // Provide an existing Environment
            "VariableName", // Provide an existing VariableName
            "NewValue" // Provide the new value for the variable
        );

        // Act
        var response = await _subject.Handle(command, CancellationToken.None);

        // Assert
        Assert.Empty(response.Errors); // Check that there are no errors in the response
        Assert.True(response.WasSuccessful); // Check that the response indicates success
        Assert.Equal(2, response.NewVersion); // Check the value of the NewVersion property (ExpectedVersion + 1)

        _documentSession.Verify(x => x.AppendToStream(command.EnvironmentSetId, command.ExpectedVersion, It.IsAny<EnvironmentSetVariableChanged>()), Times.Once); // Check that AppendToStream is called once with the correct parameters
        _documentSession.Verify(x => x.SaveChangesAsync(), Times.Once); // Check that SaveChangesAsync is called once for EnvironmentSet
    }
}