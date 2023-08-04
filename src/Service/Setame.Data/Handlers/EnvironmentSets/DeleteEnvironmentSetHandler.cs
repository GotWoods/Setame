using MediatR;
using Microsoft.Extensions.Logging;
using Setame.Data.Models;

namespace Setame.Data.Handlers.EnvironmentSets;

public record DeleteEnvironmentSet(Guid EnvironmentSetId) : IRequest<CommandResponse>;

public class DeleteEnvironmentSetHandler : IRequestHandler<DeleteEnvironmentSet, CommandResponse>
{
    private readonly IDocumentSessionHelper<EnvironmentSet> _documentSession;
    private readonly ILogger<DeleteEnvironmentSetHandler> _logger;

    public DeleteEnvironmentSetHandler(IDocumentSessionHelper<EnvironmentSet> documentSession, ILogger<DeleteEnvironmentSetHandler> logger)
    {
        _documentSession = documentSession;
        _logger = logger;
    }

    public async Task<CommandResponse> Handle(DeleteEnvironmentSet command, CancellationToken cancellationToken)
    {
        await _documentSession.AppendToStream(command.EnvironmentSetId, new EnvironmentSetDeleted(command.EnvironmentSetId));
        await _documentSession.SaveChangesAsync();
        _logger.LogDebug("Deleted environment set {EnvironmentSet}", command.EnvironmentSetId);
        return CommandResponse.FromSuccess(-1);

    }
}