using MediatR;
using Microsoft.Extensions.Logging;
using Setame.Data.Data;
using Setame.Data.Models;

namespace Setame.Data.Handlers.Applications;

public record RenameDefaultApplicationVariable
    (Guid ApplicationId, int ExpectedVersion, string OldName, string NewName) : IRequest<CommandResponse>;

public class RenameDefaultApplicationVariableHandler : IRequestHandler<RenameDefaultApplicationVariable, CommandResponse>
{
    private readonly IDocumentSessionHelper<Application> _documentSession;
    private readonly IApplicationRepository _querySession;
    private readonly ILogger<RenameApplicationVariableHandler> _logger;

    public RenameDefaultApplicationVariableHandler(IDocumentSessionHelper<Application> documentSession,
        IApplicationRepository querySession, ILogger<RenameApplicationVariableHandler> logger)
    {
        _documentSession = documentSession;
        _querySession = querySession;
        _logger = logger;
    }

    public async Task<CommandResponse> Handle(RenameDefaultApplicationVariable command, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Renaming Default Variable {Variable} to {NewName} for {Application}", command.OldName, command.NewName, command.ApplicationId);
        var response = new CommandResponse();
        var existing = await _querySession.GetById(command.ApplicationId);

        var found = existing.ApplicationDefaults.Any(x => x.Name == command.OldName);

        if (!found)
        {
            _logger.LogWarning("Could not rename {Variable} to {NewName} as the variable was not found", command.OldName, command.NewName);
            response.Errors.Add(Errors.VariableNotFoundRename(command.OldName));
            return response;
        }

        await _documentSession.AppendToStream(command.ApplicationId, command.ExpectedVersion, new ApplicationDefaultVariableRenamed(command.OldName, command.NewName));
        await _documentSession.SaveChangesAsync();
        response.NewVersion = command.ExpectedVersion + 1;
        _logger.LogDebug("Rename variable completed");
        return response;
    }
}