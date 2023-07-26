using MediatR;
using Microsoft.Extensions.Logging;
using Setame.Data.Data;
using Setame.Data.Models;

namespace Setame.Data.Handlers.Applications;

public record UpdateApplicationVariable(Guid ApplicationId, int ExpectedVersion, string Environment,
    string VariableName, string NewValue) : IRequest<CommandResponse>;

public class UpdateApplicationVariableHandler : IRequestHandler<UpdateApplicationVariable, CommandResponse>
{
    private readonly IDocumentSessionHelper<Application> _documentSession;
    private readonly IApplicationRepository _querySession;
    private readonly ILogger<UpdateApplicationVariableHandler> _logger;

    public UpdateApplicationVariableHandler(IDocumentSessionHelper<Application> documentSession, IApplicationRepository querySession, ILogger<UpdateApplicationVariableHandler> logger)
    {
        _documentSession = documentSession;
        _querySession = querySession;
        _logger = logger;
    }

    public async Task<CommandResponse> Handle(UpdateApplicationVariable command, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Updating variable {Variable} for application {Application}. Setting variable to {NewValue}.", command.VariableName, command.ApplicationId, command.NewValue);
        var response = new CommandResponse();

        var existing = await _querySession.GetById(command.ApplicationId);

        var environmentSetting = existing.EnvironmentSettings.FirstOrDefault(x=>x.Name == command.Environment);
        if (environmentSetting == null)
        {
            _logger.LogWarning("Environment {Environment} not found", command.Environment);
            response.Errors.Add(Errors.EnvironmentNotFound(command.Environment));
            return response;
        }

        if (environmentSetting.Settings.FirstOrDefault(x => x.Name == command.VariableName) == null)
        {
            _logger.LogWarning("Variable {Variable} not found", command.VariableName);
            response.Errors.Add(Errors.VariableNotFound(command.VariableName));
            return response;
        }
       

        await _documentSession.AppendToStream(command.ApplicationId, command.ExpectedVersion, new ApplicationVariableChanged(command.Environment, command.VariableName, command.NewValue));
        await _documentSession.SaveChangesAsync();
        response.NewVersion = command.ExpectedVersion + 1;
        _logger.LogDebug("Variable updated");
        return response;
    }
}