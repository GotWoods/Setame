using Microsoft.Extensions.Logging;
using Moq;
using Setame.Data;
using Setame.Data.Data;
using Setame.Data.Handlers.Applications;
using Setame.Data.Models;
using Setame.Data.Projections;

namespace Setame.Tests.Handlers.Applications;

public class CreateApplicationHandlerTests
{
    private readonly Mock<IApplicationRepository> _activeApplicationRepository;
    private readonly Mock<IDocumentSessionHelper<Application>> _applicationDocumentSession;
    private readonly Mock<IDocumentSessionHelper<EnvironmentSet>> _environmentSetDocumentSession;
    private readonly CreateApplicationHandler _subject;

    public CreateApplicationHandlerTests()
    {
        _applicationDocumentSession = new Mock<IDocumentSessionHelper<Application>>();
        _environmentSetDocumentSession = new Mock<IDocumentSessionHelper<EnvironmentSet>>();
        _activeApplicationRepository = new Mock<IApplicationRepository>();
        _subject = new CreateApplicationHandler(
            _applicationDocumentSession.Object,
            _environmentSetDocumentSession.Object,
            _activeApplicationRepository.Object,
            new Mock<ILogger<CreateApplicationHandler>>().Object
        );
    }

    [Fact]
    public async Task Handle_ValidData_SuccessfulResponse()
    {
        // Arrange
        var command = new CreateApplication(
            Guid.NewGuid(), // Provide a valid ApplicationId
            "Sample Application", // Provide a unique Name
            "Sample Token",
            Guid.NewGuid() // Provide a valid EnvironmentSetId
        );

        var environmentSet = new EnvironmentSet
        {
            Id = command.EnvironmentSetId,
            DeploymentEnvironments = new List<DeploymentEnvironment> { new() { Name = "Dev" }, new() { Name = "Prod" } }
        };

        _environmentSetDocumentSession.Setup(x => x.GetFromEventStream(It.IsAny<Guid>())).ReturnsAsync(environmentSet);
        _activeApplicationRepository.Setup(x => x.GetByName(command.Name)).Returns<ActiveApplication>(null);


        var response = await _subject.Handle(command, CancellationToken.None);


        Assert.Empty(response.Errors); // Check that there are no errors in the response
        Assert.True(response.WasSuccessful); // Check that the response indicates success
        Assert.Equal(3, response.NewVersion); // Check the value of the NewVersion property
    }

    [Fact]
    public async Task Handle_InvalidEnvironmentSet_FailureResponse()
    {
        // Arrange
        var command = new CreateApplication(
            Guid.NewGuid(),
            "Sample Application",
            "Sample Token",
            Guid.NewGuid()
        );

        _environmentSetDocumentSession.Setup(x => x.GetFromEventStream(command.EnvironmentSetId)).ReturnsAsync((EnvironmentSet)null!); // Return null to simulate an invalid EnvironmentSet

        var ex = await Assert.ThrowsAsync<NullReferenceException>(async () => await _subject.Handle(command, CancellationToken.None));


        Assert.Equal("Could not find an environment set with an id of " + command.EnvironmentSetId, ex.Message); // Check the exception message
    }

    [Fact]
    public async Task Handle_DuplicateName_FailureResponse()
    {
        // Arrange
        var command = new CreateApplication(
            Guid.NewGuid(),
            "Sample Application", // Use a duplicate name that already exists in the querySession
            "Sample Token",
            Guid.NewGuid()
        );

        var environmentSet = new EnvironmentSet
        {
            Id = command.EnvironmentSetId,
            DeploymentEnvironments = new List<DeploymentEnvironment> { new() { Name = "Dev" }, new() { Name = "Prod" } }
        };

        _environmentSetDocumentSession.Setup(x => x.GetFromEventStream(command.EnvironmentSetId)).ReturnsAsync(environmentSet);

        _activeApplicationRepository.Setup(x => x.GetByName(command.Name)).Returns<string>(_ => new ActiveApplication { Id = Guid.NewGuid(), Name = command.Name });

        var response = await _subject.Handle(command, CancellationToken.None);

        Assert.False(response.WasSuccessful); // Check that the response indicates failure
        Assert.NotEmpty(response.Errors); // Check that there are errors in the response
        Assert.Single(response.Errors); // Check that there is only one error
        Assert.Contains(response.Errors,
            e => e.ErrorCode ==
                 Errors.DuplicateName(command.Name)
                     .ErrorCode); // Check that the error message matches the expected error message for a duplicate name
    }
}