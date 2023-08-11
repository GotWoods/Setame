using Xunit;
using Moq;
using Setame.Data.Models;
using Setame.Data.Handlers.EnvironmentSets;
using System.Collections.Generic;
using System;
using Microsoft.Extensions.Logging;
using Setame.Data.Data;
using Setame.Data;

namespace Setame.Tests.Handlers.EnvironmentSets
{
    public class RenameEnvironmentSetVariableHandlerTests
    {
        private readonly Mock<IDocumentSessionHelper<EnvironmentSet>> _docSessionMock;
        private readonly Mock<IEnvironmentSetRepository> _envSetRepoMock;
        private readonly Mock<ILogger<RenameEnvironmentSetVariableHandler>> _loggerMock;
        private readonly RenameEnvironmentSetVariableHandler _handler;

        public RenameEnvironmentSetVariableHandlerTests()
        {
            _docSessionMock = new Mock<IDocumentSessionHelper<EnvironmentSet>>();
            _envSetRepoMock = new Mock<IEnvironmentSetRepository>();
            _loggerMock = new Mock<ILogger<RenameEnvironmentSetVariableHandler>>();

            _handler = new RenameEnvironmentSetVariableHandler(_docSessionMock.Object, _envSetRepoMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_NewVariableNameExists_ReturnsError()
        {
            // Arrange
            var command = new RenameEnvironmentSetVariable(Guid.NewGuid(), 1, "OldVar", "NewVar");
            var environmentSet = new EnvironmentSet
            {
                Environments = new List<DeploymentEnvironment>
                {
                    new DeploymentEnvironment
                    {
                        Settings = new Dictionary<string, string>() {{ "NewVar", "NewVar" }}
                    }
                }
            };

            _envSetRepoMock.Setup(r => r.GetById(command.EnvironmentSetId)).ReturnsAsync(environmentSet);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.WasSuccessful);
        }

        [Fact]
        public async Task Handle_NewVariableNameDoesNotExist_AppendsEventAndReturnsSuccess()
        {
            // Arrange
            var command = new RenameEnvironmentSetVariable(Guid.NewGuid(), 1, "OldVar", "NewVar");
            var environmentSet = new EnvironmentSet { Environments = new List<DeploymentEnvironment>() };

            _envSetRepoMock.Setup(r => r.GetById(command.EnvironmentSetId)).ReturnsAsync(environmentSet);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.WasSuccessful);
            Assert.Equal(2, result.NewVersion);

            _docSessionMock.Verify(d => d.AppendToStream(command.EnvironmentSetId, command.ExpectedVersion, It.IsAny<EnvironmentSetVariableRenamed>()), Times.Once);
            _docSessionMock.Verify(d => d.SaveChangesAsync(), Times.Once);
        }
    }
}
