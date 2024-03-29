﻿using Microsoft.Extensions.Logging;
using Moq;
using Setame.Data;
using Setame.Data.Handlers.Applications;
using Setame.Data.Models;

namespace Setame.Tests.Handlers.Applications;

public class DeleteApplicationHandlerTests
{
    private readonly Mock<IDocumentSessionHelper<Application>> _documentSession;
    private readonly DeleteApplicationHandler _subject;

    public DeleteApplicationHandlerTests()
    {
        _documentSession = new Mock<IDocumentSessionHelper<Application>>();
        _subject = new DeleteApplicationHandler(_documentSession.Object, new Mock<ILogger<DeleteApplicationHandler>>().Object);
    }

    [Fact]
    public async Task Handle_ValidData_SuccessfulDeletion()
    {
        // Arrange
        var command = new DeleteApplication(Guid.NewGuid()); // Provide a valid ApplicationId

        // Act
        await _subject.Handle(command, CancellationToken.None);

        // Assert
        _documentSession.Verify(x => x.AppendToStream(command.ApplicationId, It.IsAny<ApplicationDeleted>()), Times.Once); // Check that AppendToStream is called once with the correct parameters
        _documentSession.Verify(x => x.SaveChangesAsync(), Times.Once); // Check that SaveChangesAsync is called once
    }
}