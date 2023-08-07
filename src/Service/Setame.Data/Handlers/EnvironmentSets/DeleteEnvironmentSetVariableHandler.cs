using MediatR;
using Microsoft.Extensions.Logging;
using Setame.Data.Models;

namespace Setame.Data.Handlers.EnvironmentSets;

public record DeleteEnvironmentSetVariable(Guid EnvironmentSetId, int ExpectedVersion, string VariableName) : IRequest<CommandResponse>;

public class DeleteEnvironmentSetVariableHandler : IRequestHandler<DeleteEnvironmentSetVariable, CommandResponse>
{
    private readonly IDocumentSessionHelper<EnvironmentSet> _documentSession;
    private readonly ILogger<UpdateEnvironmentSetVariableHandler> _logger;

    public DeleteEnvironmentSetVariableHandler(IDocumentSessionHelper<EnvironmentSet> documentSession, ILogger<UpdateEnvironmentSetVariableHandler> logger)
    {
        _documentSession = documentSession;
        _logger = logger;
    }


    public async Task<CommandResponse> Handle(DeleteEnvironmentSetVariable command, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Deleting {VariableName} from environment set {EnvironmentSetId}", command.VariableName, command.EnvironmentSetId);
        await _documentSession.AppendToStream(command.EnvironmentSetId, command.ExpectedVersion, new EnvironmentSetVariableDeleted(command.VariableName));
        await _documentSession.SaveChangesAsync();
        _logger.LogDebug("variable deleted");
        return CommandResponse.FromSuccess(command.ExpectedVersion + 1);
    }
}