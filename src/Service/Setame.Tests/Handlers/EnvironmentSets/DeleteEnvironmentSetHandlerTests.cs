using Xunit;
using Moq;
using Setame.Data.Models;
using Setame.Data.Handlers.EnvironmentSets;
using System;
using Microsoft.Extensions.Logging;
using Setame.Data;

namespace Setame.Tests.Handlers.EnvironmentSets
{
    public class DeleteEnvironmentSetHandlerTests
    {
        private readonly Mock<IDocumentSessionHelper<EnvironmentSet>> _documentSessionMock;
        private readonly Mock<ILogger<DeleteEnvironmentSetHandler>> _loggerMock;
        private readonly DeleteEnvironmentSetHandler _handler;

        public DeleteEnvironmentSetHandlerTests()
        {
            _documentSessionMock = new Mock<IDocumentSessionHelper<EnvironmentSet>>();
            _loggerMock = new Mock<ILogger<DeleteEnvironmentSetHandler>>();
            _handler = new DeleteEnvironmentSetHandler(_documentSessionMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ValidCommand_DeletesEnvironmentSetSuccessfully()
        {
            // Arrange
            var command = new DeleteEnvironmentSet(Guid.NewGuid());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            _documentSessionMock.Verify(d => d.AppendToStream(command.EnvironmentSetId, It.IsAny<EnvironmentSetDeleted>()), Times.Once);
            _documentSessionMock.Verify(d => d.SaveChangesAsync(), Times.Once);

            Assert.True(result.WasSuccessful);
            Assert.Equal(-1, result.NewVersion);
        }
    }
}