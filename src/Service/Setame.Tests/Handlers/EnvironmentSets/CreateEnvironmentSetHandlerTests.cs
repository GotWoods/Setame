using JasperFx.Core;
using Microsoft.Extensions.Logging;
using Moq;
using Setame.Data;
using Setame.Data.Data;
using Setame.Data.Handlers.EnvironmentSets;
using Setame.Data.Models;

namespace Setame.Tests.Handlers.EnvironmentSets;

public class CreateEnvironmentSetHandlerTests
{
    private readonly Mock<IDocumentSessionHelper<EnvironmentSet>> _documentSession;
    private readonly Mock<IEnvironmentSetRepository> _environmentSetRepository;
    private readonly CreateEnvironmentSetHandler _subject;

    public CreateEnvironmentSetHandlerTests()
    {
        _documentSession = new Mock<IDocumentSessionHelper<EnvironmentSet>>();
        _environmentSetRepository = new Mock<IEnvironmentSetRepository>();
        _subject = new CreateEnvironmentSetHandler(_documentSession.Object, _environmentSetRepository.Object, new Mock<ILogger<CreateEnvironmentSetHandler>>().Object);
    }

    [Fact]
    public async Task Handle_ValidData_SuccessfulCreation()
    {
        // Arrange
        var command = new CreateEnvironmentSet("Sample EnvironmentSet"); // Provide a unique environment set name

        _environmentSetRepository.Setup(x => x.GetByName(command.Name)).Returns<EnvironmentSet>(null); // Return null to simulate that the environment set name is not found (unique)

        // Act
        var response = await _subject.Handle(command, CancellationToken.None);

        // Assert
        Assert.Empty(response.Errors); // Check that there are no errors in the response
        Assert.True(response.WasSuccessful); // Check that the response indicates success
        Assert.Equal(1, response.NewVersion); // Check the value of the NewVersion property
        _documentSession.Verify(x => x.Start(It.IsAny<Guid>(), It.IsAny<EnvironmentSetCreated>()), Times.Once); // Check that Start is called once with the correct parameters
        _documentSession.Verify(x => x.SaveChangesAsync(), Times.Once); // Check that SaveChangesAsync is called once
    }

    [Fact]
    public async Task Handle_DuplicateEnvironmentSet_FailureResponse()
    {
        // Arrange
        var command = new CreateEnvironmentSet("Sample EnvironmentSet"); // Provide a duplicate environment set name

        var existingEnvironmentSet = new EnvironmentSet
        {
            Id = CombGuidIdGeneration.NewGuid(),
            Name = command.Name // Set the name to match the command (simulating a duplicate name)
        };

        _environmentSetRepository.Setup(x => x.GetByName(command.Name)).Returns(existingEnvironmentSet); // Return an environment set with the same name

        // Act
        var response = await _subject.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotEmpty(response.Errors); // Check that there are errors in the response
        Assert.Contains(response.Errors, e => e.ErrorCode == Errors.DuplicateName(command.Name).ErrorCode); // Check that the error message matches the expected error message for duplicate name
    }
}