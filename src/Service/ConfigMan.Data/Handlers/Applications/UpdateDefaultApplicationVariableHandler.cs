using ConfigMan.Data.Models;
using Marten;
using MediatR;

namespace ConfigMan.Data.Handlers.Applications;

public record UpdateDefaultApplicationVariable(Guid ApplicationId, int ExpectedVersion, string VariableName,
    string NewValue) : IRequest<CommandResponse>;

public class
    UpdateDefaultApplicationVariableHandler : IRequestHandler<UpdateDefaultApplicationVariable, CommandResponse>
{
    private readonly IDocumentSessionHelper<Application> _documentSession;
    private readonly IQuerySession _querySession;

    public UpdateDefaultApplicationVariableHandler(IDocumentSessionHelper<Application> documentSession,
        IQuerySession querySession)
    {
        _documentSession = documentSession;
        _querySession = querySession;
    }

    public async Task<CommandResponse> Handle(UpdateDefaultApplicationVariable command,
        CancellationToken cancellationToken)
    {
        var response = new CommandResponse();

        var existing =
            await _querySession.Events.AggregateStreamAsync<Application>(command.ApplicationId,
                token: cancellationToken);
        if (existing == null)
        {
            response.Errors.Add(Errors.ApplicationNotFound(command.ApplicationId));
            return response;
        }

        if (existing.ApplicationDefaults.FirstOrDefault(x => x.Name == command.VariableName) == null)
        {
            response.Errors.Add(Errors.VariableNotFound(command.VariableName));
            return response;
        }

        await _documentSession.AppendToStream(command.ApplicationId, command.ExpectedVersion, new ApplicationDefaultVariableChanged(command.VariableName, command.NewValue));
        await _documentSession.SaveChangesAsync();
        response.NewVersion = command.ExpectedVersion + 1;
        return response;
    }
}