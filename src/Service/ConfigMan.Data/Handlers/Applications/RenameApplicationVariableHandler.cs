using ConfigMan.Data.Models;
using Marten;
using Marten.Internal.Sessions;
using MediatR;

namespace ConfigMan.Data.Handlers.Applications;

public record RenameApplicationVariable(Guid ApplicationId, int ExpectedVersion, string OldName, string NewName) : IRequest<CommandResponse>;
public class RenameApplicationVariableHandler : IRequestHandler<RenameApplicationVariable, CommandResponse>
{
    private readonly IDocumentSessionHelper<Application> _documentSession;
    private readonly IQuerySession _querySession;

    public RenameApplicationVariableHandler(IDocumentSessionHelper<Application> documentSession, IQuerySession querySession)
    {
        _documentSession = documentSession;
        _querySession = querySession;
    }

    public async Task<CommandResponse> Handle(RenameApplicationVariable command, CancellationToken cancellationToken)
    {
        var response = new CommandResponse();
        var existing = await _querySession.Events.AggregateStreamAsync<Application>(command.ApplicationId, token: cancellationToken);
        if (existing != null)
        {
            response.Errors.Add(Errors.ApplicationNotFound(command.ApplicationId));
            return response;
        }

        var found = false;
        foreach (var environmentSetting in existing.EnvironmentSettings)
        {
            if (environmentSetting.Settings.Any(x => x.Name == command.OldName))
            {
                found = true;
                break;
            }
        }

        if (!found)
        {
            response.Errors.Add(Errors.VariableNotFoundRename(command.OldName));
            return response;
        }

        await _documentSession.AppendToStream(command.ApplicationId, command.ExpectedVersion, new ApplicationVariableRenamed(command.OldName, command.NewName));
        await _documentSession.SaveChangesAsync();
        response.NewVersion = command.ExpectedVersion + 1;
        return response;

    }
}