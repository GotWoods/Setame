using ConfigMan.Data.Data;
using ConfigMan.Data.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ConfigMan.Data.Handlers.Applications;

public record UpdateDefaultApplicationVariable(Guid ApplicationId, int ExpectedVersion, string VariableName,
    string NewValue) : IRequest<CommandResponse>;

public class
    UpdateDefaultApplicationVariableHandler : IRequestHandler<UpdateDefaultApplicationVariable, CommandResponse>
{
    private readonly IDocumentSessionHelper<Application> _documentSession;
    private readonly IApplicationRepository _querySession;
    private readonly ILogger<UpdateDefaultApplicationVariableHandler> _logger;

    public UpdateDefaultApplicationVariableHandler(IDocumentSessionHelper<Application> documentSession,
        IApplicationRepository querySession, ILogger<UpdateDefaultApplicationVariableHandler> logger)
    {
        _documentSession = documentSession;
        _querySession = querySession;
        _logger = logger;
    }

    public async Task<CommandResponse> Handle(UpdateDefaultApplicationVariable command,
        CancellationToken cancellationToken)
    {
        var response = new CommandResponse();

        var existing =
            await _querySession.GetById(command.ApplicationId);
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

        await _documentSession.AppendToStream(command.ApplicationId, command.ExpectedVersion,
            new ApplicationDefaultVariableChanged(command.VariableName, command.NewValue));
        await _documentSession.SaveChangesAsync();
        response.NewVersion = command.ExpectedVersion + 1;
        return response;
    }
}