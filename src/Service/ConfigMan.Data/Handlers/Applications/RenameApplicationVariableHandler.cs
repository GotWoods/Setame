using ConfigMan.Data.Data;
using ConfigMan.Data.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ConfigMan.Data.Handlers.Applications;

public record RenameApplicationVariable
    (Guid ApplicationId, int ExpectedVersion, string OldName, string NewName) : IRequest<CommandResponse>;

public class RenameApplicationVariableHandler : IRequestHandler<RenameApplicationVariable, CommandResponse>
{
    private readonly IDocumentSessionHelper<Application> _documentSession;
    private readonly IApplicationRepository _querySession;
    private readonly ILogger<RenameApplicationVariableHandler> _logger;

    public RenameApplicationVariableHandler(IDocumentSessionHelper<Application> documentSession,
        IApplicationRepository querySession, ILogger<RenameApplicationVariableHandler> logger)
    {
        _documentSession = documentSession;
        _querySession = querySession;
        _logger = logger;
    }

    public async Task<CommandResponse> Handle(RenameApplicationVariable command, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Renaming Variable {Variable} to {NewName} for {Application}", command.OldName, command.NewName, command.ApplicationId);
        var response = new CommandResponse();
        var existing = await _querySession.GetById(command.ApplicationId);
        if (existing == null)
            throw new NullReferenceException("Application not found");

        var found = false;
        foreach (var environmentSetting in existing.EnvironmentSettings)
            if (environmentSetting.Settings.Any(x => x.Name == command.OldName))
            {
                found = true;
                break;
            }

        if (!found)
        {
            _logger.LogWarning("Could not rename {Variable} to {NewName} as the variable was not found", command.OldName, command.NewName);
            response.Errors.Add(Errors.VariableNotFoundRename(command.OldName));
            return response;
        }

        await _documentSession.AppendToStream(command.ApplicationId, command.ExpectedVersion, new ApplicationVariableRenamed(command.OldName, command.NewName));
        await _documentSession.SaveChangesAsync();
        response.NewVersion = command.ExpectedVersion + 1;
        _logger.LogDebug("Rename variable completed");
        return response;
    }
}