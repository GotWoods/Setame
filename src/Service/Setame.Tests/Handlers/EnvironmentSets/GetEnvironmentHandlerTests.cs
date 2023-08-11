using Xunit;
using Moq;
using Setame.Data.Models;
using Setame.Data.Handlers.EnvironmentSets;
using System;
using Setame.Data.Data;

namespace Setame.Tests.Handlers.EnvironmentSets
{
    public class GetEnvironmentHandlerTests
    {
        private readonly Mock<IEnvironmentSetRepository> _repositoryMock;
        private readonly GetEnvironmentHandler _handler;

        public GetEnvironmentHandlerTests()
        {
            _repositoryMock = new Mock<IEnvironmentSetRepository>();

            _handler = new GetEnvironmentHandler(_repositoryMock.Object);
        }

        [Fact]
        public async Task Handle_EnvironmentExists_ReturnsEnvironmentSet()
        {
            // Arrange
            var environmentSetId = Guid.NewGuid();
            var environmentSet = new EnvironmentSet();  // Assuming a default constructor for EnvironmentSet

            _repositoryMock.Setup(r => r.GetById(environmentSetId))
                .ReturnsAsync(environmentSet);

            var command = new GetEnvironment(environmentSetId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Same(environmentSet, result);
        }

        [Fact]
        public async Task Handle_EnvironmentDoesNotExist_ThrowsException()
        {
            // Arrange
            var environmentSetId = Guid.NewGuid();

            _repositoryMock.Setup(r => r.GetById(environmentSetId))
                .ReturnsAsync((EnvironmentSet)null);

            var command = new GetEnvironment(environmentSetId);

            // Act & Assert
            await Assert.ThrowsAsync<NullReferenceException>(() => _handler.Handle(command, CancellationToken.None));
        }
    }
}