using Xunit;
using Moq;
using Setame.Data.Models;
using Setame.Data.Handlers.EnvironmentSets;
using System.Collections.Generic;
using System;
using Microsoft.Extensions.Logging;
using Setame.Data.Data;
using Setame.Data.Projections;
using Setame.Data;

namespace Setame.Tests.Handlers.EnvironmentSets
{
    public class AddEnvironmentToEnvironmentSetHandlerTests
    {
        private readonly Mock<IDocumentSessionHelper<EnvironmentSet>> _environmentSetDocSessionMock;
        private readonly Mock<IDocumentSessionHelper<Application>> _applicationDocSessionMock;
        private readonly Mock<IEnvironmentSetRepository> _envSetRepoMock;
        private readonly Mock<IEnvironmentSetApplicationAssociationRepository> _assocRepoMock;
        private readonly Mock<ILogger<AddEnvironmentToEnvironmentSet>> _loggerMock;

        private readonly AddEnvironmentToEnvironmentSetHandler _handler;

        public AddEnvironmentToEnvironmentSetHandlerTests()
        {
            _environmentSetDocSessionMock = new Mock<IDocumentSessionHelper<EnvironmentSet>>();
            _applicationDocSessionMock = new Mock<IDocumentSessionHelper<Application>>();
            _envSetRepoMock = new Mock<IEnvironmentSetRepository>();
            _assocRepoMock = new Mock<IEnvironmentSetApplicationAssociationRepository>();
            _loggerMock = new Mock<ILogger<AddEnvironmentToEnvironmentSet>>();

            _handler = new AddEnvironmentToEnvironmentSetHandler(_environmentSetDocSessionMock.Object, _applicationDocSessionMock.Object, _envSetRepoMock.Object, _assocRepoMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_EnvironmentNameExists_ReturnsError()
        {
            // Arrange
            var command = new AddEnvironmentToEnvironmentSet(Guid.NewGuid(), 1, "TestEnv");
            var environmentSet = new EnvironmentSet { Environments = new List<DeploymentEnvironment> { new DeploymentEnvironment { Name = "TestEnv" } } };

            _envSetRepoMock.Setup(r => r.GetById(command.EnvironmentSetId)).ReturnsAsync(environmentSet);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.WasSuccessful);
        }

        [Fact]
        public async Task Handle_ValidEnvironment_AppendsEventsAndReturnsSuccess()
        {
            // Arrange
            var command = new AddEnvironmentToEnvironmentSet(Guid.NewGuid(), 1, "TestEnv");
            var environmentSet = new EnvironmentSet { Environments = new List<DeploymentEnvironment>() };
            var association = new EnvironmentSetApplicationAssociation
            {
                Id = command.EnvironmentSetId,
                Applications = new List<SimpleApplication> { new SimpleApplication(Guid.NewGuid() , "") }
            };

            _envSetRepoMock.Setup(r => r.GetById(command.EnvironmentSetId)).ReturnsAsync(environmentSet);
            _assocRepoMock.Setup(a => a.Get(command.EnvironmentSetId)).Returns(association);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.WasSuccessful);
            Assert.Equal(2, result.NewVersion);

            _environmentSetDocSessionMock.Verify(d => d.AppendToStream(command.EnvironmentSetId, command.ExpectedVersion, It.IsAny<EnvironmentAdded>()), Times.Once);
            _applicationDocSessionMock.Verify(d => d.AppendToStream(It.IsAny<Guid>(), It.IsAny<ApplicationEnvironmentAdded>()), Times.Exactly(association.Applications.Count));

            _environmentSetDocSessionMock.Verify(d => d.SaveChangesAsync(), Times.Once);
            _applicationDocSessionMock.Verify(d => d.SaveChangesAsync(), Times.Once);

        }
    }
}
