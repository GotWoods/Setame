using Microsoft.Extensions.Logging;
using Xunit;
using Moq;
using Setame.Data;
using Setame.Data.Models;
using Setame.Data.Handlers.Applications;

namespace Setame.Tests.Handlers.Applications
{
    public class DeleteApplicationVariableHandlerTests
    {
        private readonly Mock<IDocumentSessionHelper<Application>> _documentSessionMock;
        private readonly Mock<ILogger<UpdateApplicationVariableHandler>> _loggerMock;
        private readonly DeleteApplicationVariableHandler _handler;

        public DeleteApplicationVariableHandlerTests()
        {
            _documentSessionMock = new Mock<IDocumentSessionHelper<Application>>();
            _loggerMock = new Mock<ILogger<UpdateApplicationVariableHandler>>();
            _handler = new DeleteApplicationVariableHandler(
                _documentSessionMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ValidCommand_VariableDeletedSuccessfully()
        {
            // Arrange
            var command = new DeleteApplicationVariable(Guid.NewGuid(), 1, "VariableName");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Empty(result.Errors);
            _documentSessionMock.Verify(d => d.AppendToStream(
                    command.ApplicationId,
                    command.ExpectedVersion,
                    It.IsAny<ApplicationVariableDeleted>()),
                Times.Once);
            _documentSessionMock.Verify(d => d.SaveChangesAsync(), Times.Once);
            Assert.Equal(command.ExpectedVersion + 1, result.NewVersion);
        }
    }
}