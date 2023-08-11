using Xunit;
using Moq;
using System.Collections.Generic;
using Setame.Data.Models;
using Setame.Data.Data;
using Setame.Data.Handlers.Applications;
using Microsoft.Extensions.Logging;
using Setame.Data;

namespace Setame.Tests.Handlers.Applications
{
    public class RenameDefaultApplicationVariableHandlerTests
    {
        private readonly Mock<IDocumentSessionHelper<Application>> _documentSessionMock;
        private readonly Mock<IApplicationRepository> _querySessionMock;
        private readonly Mock<ILogger<RenameApplicationVariableHandler>> _loggerMock;
        private readonly RenameDefaultApplicationVariableHandler _handler;

        public RenameDefaultApplicationVariableHandlerTests()
        {
            _documentSessionMock = new Mock<IDocumentSessionHelper<Application>>();
            _querySessionMock = new Mock<IApplicationRepository>();
            _loggerMock = new Mock<ILogger<RenameApplicationVariableHandler>>();
            _handler = new RenameDefaultApplicationVariableHandler(
                _documentSessionMock.Object,
                _querySessionMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_VariableNotFound_ReturnsError()
        {
            // Arrange
            var command = new RenameDefaultApplicationVariable(Guid.NewGuid(), 1, "OldVariable", "NewVariable");
            var existingApp = new Application
            {
                ApplicationDefaults = new List<Setting>() // Empty list for this test
            };
            _querySessionMock.Setup(s => s.GetById(command.ApplicationId)).ReturnsAsync(existingApp);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotEmpty(result.Errors);
            Assert.Contains(result.Errors, e => e.ErrorCode == Errors.VariableNotFoundRename(command.OldName).ErrorCode);
        }

        [Fact]
        public async Task Handle_VariableFound_RenamesSuccessfully()
        {
            // Arrange
            var command = new RenameDefaultApplicationVariable(Guid.NewGuid(), 1, "OldVariable", "NewVariable");
            var existingApp = new Application
            {
                ApplicationDefaults = new List<Setting>
                {
                    new Setting { Name = "OldVariable" }
                }
            };
            _querySessionMock.Setup(s => s.GetById(command.ApplicationId)).ReturnsAsync(existingApp);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Empty(result.Errors);
            _documentSessionMock.Verify(d => d.AppendToStream(
                command.ApplicationId,
                command.ExpectedVersion,
                It.IsAny<ApplicationDefaultVariableRenamed>()),
                Times.Once);
            _documentSessionMock.Verify(d => d.SaveChangesAsync(), Times.Once);
            Assert.Equal(command.ExpectedVersion + 1, result.NewVersion);
        }
    }
}
