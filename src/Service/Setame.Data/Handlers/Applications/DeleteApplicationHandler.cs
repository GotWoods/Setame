using MediatR;
using Microsoft.Extensions.Logging;
using Setame.Data.Models;

namespace Setame.Data.Handlers.Applications;

public record DeleteApplication(Guid ApplicationId) : IRequest;

public class DeleteApplicationHandler : IRequestHandler<DeleteApplication>
{
    private readonly IDocumentSessionHelper<Application> _documentSession;
    private readonly ILogger<DeleteApplicationHandler> _logger;

    public DeleteApplicationHandler(IDocumentSessionHelper<Application> documentSession, ILogger<DeleteApplicationHandler> logger)
    {
        _documentSession = documentSession;
        _logger = logger;
    }

    public async Task Handle(DeleteApplication command, CancellationToken cancellationToken)
    {
        await _documentSession.AppendToStream(command.ApplicationId, new ApplicationDeleted(command.ApplicationId));
        await _documentSession.SaveChangesAsync();
        _logger.LogDebug("Deleted application {Application}", command.ApplicationId);
    }
}