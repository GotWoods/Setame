using Xunit;
using Moq;
using Setame.Data.Models;
using Setame.Data.Handlers.EnvironmentSets;
using System;
using Microsoft.Extensions.Logging;
using Setame.Data;

namespace Setame.Tests.Handlers.EnvironmentSets
{
    public class DeleteEnvironmentSetVariableHandlerTests
    {
        private readonly Mock<IDocumentSessionHelper<EnvironmentSet>> _docSessionMock;
        private readonly Mock<ILogger<UpdateEnvironmentSetVariableHandler>> _loggerMock;
        private readonly DeleteEnvironmentSetVariableHandler _handler;

        public DeleteEnvironmentSetVariableHandlerTests()
        {
            _docSessionMock = new Mock<IDocumentSessionHelper<EnvironmentSet>>();
            _loggerMock = new Mock<ILogger<UpdateEnvironmentSetVariableHandler>>();

            _handler = new DeleteEnvironmentSetVariableHandler(_docSessionMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_DeleteVariable_AppendsEventAndReturnsSuccess()
        {
            // Arrange
            var command = new DeleteEnvironmentSetVariable(Guid.NewGuid(), 1, "SomeVariable");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.WasSuccessful);
            Assert.Equal(2, result.NewVersion);

            _docSessionMock.Verify(d => d.AppendToStream(command.EnvironmentSetId, command.ExpectedVersion, It.IsAny<EnvironmentSetVariableDeleted>()), Times.Once);
            _docSessionMock.Verify(d => d.SaveChangesAsync(), Times.Once);
        }
    }
}
